using Footlocker;
using Footlocker.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace P44ServiceLogEMailAlert
{
    
    public class FileWatcher
    {
        Timer timer;
        public string P44LogFilePath { get; private set; }
        private string Server { get; set; }
        private string Region { get; set; }
        private string FilePath { get; set; }
        private string LogFilePath { get; set; }
        private string EMailSubject { get; set; }
        private string EMailAlert { get; set; }
        private string EMailError { get; set; }
        public bool blnIsConfigRead { get; set; }
        private string ExMessage { get; set; }
        private double? IntervalInMins { get; set; }

        P44LogEMailAlert objEMailAlert;
        FLLogger _logger;
        EMail eMail;
        public FileWatcher()
        {
            timer = new Timer();
            blnIsConfigRead = ReadConfiguration();
            _logger = new FLLogger(LogFilePath);
        }
        private bool ReadConfiguration()
        {
            bool blnResult = false;
            try
            {
                Server = ConfigurationManager.AppSettings["Server"];
                Region = ConfigurationManager.AppSettings["Region"];
                FilePath = ConfigurationManager.AppSettings["FileWatcherSourceFolder"];
                LogFilePath = ConfigurationManager.AppSettings["FileWatcherLog"];
                IntervalInMins = int.Parse(ConfigurationManager.AppSettings["IntervalInMins"]);
                blnResult = true;
            }
            catch(Exception ex)
            {
                LogErrorMessage(ex);
            }
            return blnResult;
        }
        public void On_Start()
        {
            WriteToFile("Service is started at : " + DateTime.Now,FLLogger.eLogMessageType.eInfo);
            Console.WriteLine("Service is started at :  " + DateTime.Now);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            if (IntervalInMins == null)
                timer.Interval = 60000 * 60;
            else
                timer.Interval = (double)(60000 * IntervalInMins);
            //number in milisecinds
            timer.Enabled = true;
            //\log\CriticalProcessLogs\OBF2Prj44_
            //$"\\\\ordcrlprdweb05\\Log\\CriticalProcessLogs\\"
            var watcher = new FileSystemWatcher($"\\\\" + Server + FilePath)
            {
                NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size
            };

            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;
            watcher.Error += OnError;

            watcher.Filter = "*.log";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
            Console.WriteLine($"Please wait for next {IntervalInMins}  mins. to get E Mail Alert !!");
        }
        private void OnElapsedTime(object sender, ElapsedEventArgs e)
        {
            try
            {
                WriteToFile("File Watcher Service is recall at : " + DateTime.Now, FLLogger.eLogMessageType.eInfo);
                Console.WriteLine("Service is recall at : " + DateTime.Now);
                objEMailAlert = new P44LogEMailAlert();
                bool result = objEMailAlert.GenerateEMailAlert(LogFilePath, Server, Region, P44LogFilePath, EMailSubject, string.Empty);
                if (result == true)
                    Console.WriteLine("File Watcher Service, Log Information E Mail Alert End Successfully !");
                else
                    Console.WriteLine("File Watcher Service, Log Information E Mail Alert End with Failure !!!");

            }
            catch (Exception ex)
            {
                LogErrorMessage(ex);
            }
        }
        private void OnError(object sender, ErrorEventArgs e)
        {
            try
            {
                //WriteToFile(e.GetException());
                PrintException(e.GetException());
                EMailSubject = $": from P44 Service File Watcher: On Error Event !! ";
                eMail = new EMail(LogFilePath);
                string EMailBody = GetException(e.GetException());
                eMail.SendErrorEMail(EMailSubject, "\n {Region}:Region \n from Server:{Server}, \n generated at TimeStamp {DateTime.UtcNow.ToString()} \n" + EMailBody);
            }
            catch(Exception ex)
            {
                LogErrorMessage(ex);
            }
        }
        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            try
            {
                Console.WriteLine($"Renamed:");
                Console.WriteLine($"    Old: {e.OldFullPath}");
                Console.WriteLine($"    New: {e.FullPath}");
                EMailSubject = $": from P44 Service File Watcher : File Renamed {e.OldFullPath}";
                eMail = new EMail(LogFilePath);
                string EMailBody = $"Log File has been Renamed:\n Old File : {e.OldFullPath} \n to New File: {e.FullPath} \n for {Region}:Region from \n Server:{Server}, \n generated at TimeStamp {DateTime.UtcNow.ToString()}";
                eMail.SendErrorEMail(EMailSubject, EMailBody);
            }
            catch(Exception ex)
            {
                LogErrorMessage(ex);
            }
           
        }
        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            try
            {
                Console.WriteLine($"Deleted: {e.FullPath}");
                EMailSubject = $": from P44 Log : {e.FullPath}";
                eMail = new EMail(LogFilePath);
                string EMailBody = $" File has been Deleted File : {e.FullPath} \n for {Region} : Region from \n Server : {Server}, \n generated at TimeStamp {DateTime.UtcNow.ToString()}";
                eMail.SendErrorEMail(EMailSubject, EMailBody);
            }
            catch (Exception ex)
            {
                LogErrorMessage(ex);
            }
            
        }
        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            string value = $"Created: {e.FullPath}";
            Console.WriteLine(value);
            try
            {
                EMailSubject = $": from P44 Log File Watcher Service : File Created: {e.FullPath} ";
                eMail = new EMail(LogFilePath);
                string EMailBody = $"Log File has been Created File : {e.FullPath} \n for {Region}:Region from Server:{Server}, \n generated at TimeStamp {DateTime.UtcNow.ToString()}";
                eMail.SendErrorEMail(EMailSubject, EMailBody);
            }
            catch(Exception ex)
            {
                LogErrorMessage(ex);
            }
           
        }
        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            else
            {
                P44LogFilePath = e.FullPath;
            }
        }
        private void PrintException(Exception ex)
        {
            if (ex != null)
            {
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                PrintException(ex.InnerException);
            }
           
        }
        private string GetException(Exception ex)
        {
            string strException = string.Empty;
            if (ex != null)
            {
                strException = $"Message: {ex.Message}";
                strException = strException + $"Stacktrace: {ex.StackTrace}";
                strException = strException + GetException(ex.InnerException);
            }
            return strException;
        }
        public void LogErrorMessage(Exception ex)
        {
            ExMessage = string.Empty;
            if (ex != null)
            {
                ExMessage = $"Message: {ex.Message}";
                ExMessage += $"Stacktrace: {ex.StackTrace}";
                if(ex.InnerException !=null)
                    ExMessage += $"InnerException: {ex.InnerException}";

                WriteToFile(ExMessage,FLLogger.eLogMessageType.eError);
            }
        }
        public void WriteToFile(string Message, FLLogger.eLogMessageType logMessageType)
        {
            //string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(LogFilePath))
            {
                Directory.CreateDirectory(LogFilePath);
            }
            //FEDX0006_20240516.log
            string filepath = LogFilePath + "\\FileWatcherLog_" + DateTime.UtcNow.ToString("yyyyMMdd") + ".log";
             //AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    //sw.WriteLine(Message);
                }
                _logger.Log(Message, logMessageType);
            }
            else
            {
                //using (StreamWriter sw = File.AppendText(filepath))
                //{
                //    sw.WriteLine(Message);
                //}
                _logger.Log(Message, logMessageType);
            }
        }
    }
}
