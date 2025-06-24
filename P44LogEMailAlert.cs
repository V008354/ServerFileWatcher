using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Footlocker;

namespace P44ServiceLogEMailAlert
{
   
    public class P44LogEMailAlert
    {
        /// <summary>
        /// 
        /// </summary>
        public string P44LogFilePath = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public bool blnEMailNotification = false;
        /// <summary>
        /// 
        /// </summary>
        //string Server = ConfigurationManager.AppSettings["Server"];
        //string Region = ConfigurationManager.AppSettings["Region"];
        //string FilePath = ConfigurationManager.AppSettings["P44LogFilePath"];
        //string Log = ConfigurationManager.AppSettings["P44EMailAlertLog"];
        EMail eMail;
        FLLogger _logger;
        public P44LogEMailAlert()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool GenerateEMailAlert(string Log,string Server, string Region, string FilePath,string EMailSubject,string Message)
        {
            bool result = false;
            string ErrorMsg = string.Empty;
            _logger = new FLLogger(Log);
            eMail = new EMail(Log);
            //"P44 Service, Log Information E Mail Alert !
            _logger.Log($" Calling process P44 Service, Log Information E-Mail Alert !", FLLogger.eLogMessageType.eInfo);
            try
            {
                if (!Directory.Exists(Log))
                {
                    Directory.CreateDirectory(Log);
                }
                
                 //P44LogFilePath = Path.GetFullPath("\\\\" + Server + FilePath + DateTime.UtcNow.ToString("yyyyMMdd") + ".log");
                _logger.Log($" Server: {Server}, Region: {Region}, P44LogFilePath : {P44LogFilePath} ", FLLogger.eLogMessageType.eInfo);
                if (File.Exists(FilePath))
                {
                    using (StreamReader reader = new StreamReader(new FileStream(FilePath,
                        FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                    {
                        //start at the end of the file
                        StringBuilder strLine = new StringBuilder();

                        strLine.AppendLine($"P44 Service Log Check for Region:{Region} from Server:{Server}, log text copied at TimeStamp {DateTime.UtcNow.ToString()} :\n ");
                        _logger.Log($"P44 Service Log Check for Region:{Region} from Server:{Server}, reading log text at TimeStamp {DateTime.UtcNow.ToString()} ", FLLogger.eLogMessageType.eInfo);
                        
                        string tempText = reader.ReadToEnd();

                        string[] lines = tempText.Split('\n');  //this will break all string for each line
                        lines = tempText.Split('\n');
                       
                        if (lines.Length > 35)
                        {
                            for (int i = lines.Length - 35; i < lines.Length; i++)
                            {
                                //Console.WriteLine(lines[i]);
                                //strLine.AppendLine(lines[i].ToString());
                                strLine.Append(lines[i].ToString());
                            }
                        }
                        else
                        {
                            for (int i = lines.Length - 1; i < lines.Length; i++)
                            {
                                //Console.WriteLine(lines[i]);
                                //strLine.AppendLine(lines[i].ToString());
                                strLine.Append(lines[i].ToString());
                            }
                        }

                        if (!string.IsNullOrEmpty(strLine.ToString()))
                        {
                            //chwlogisticsweb@footlocker.com
                           
                            _logger.Log($"P44 Log Check for {Region}:Region & Server:{Server}, Sending Email Alert with last 35 lines log text at TimeStamp {DateTime.UtcNow.ToString()} \n", FLLogger.eLogMessageType.eInfo);
                            EMailSubject = $": from P44 Service Log Check for {Region}:Region from Server:{Server}, generated at TimeStamp {DateTime.UtcNow.ToString()}";
                            eMail.SendEmailAlert(EMailSubject, strLine.ToString());
                            

                         }
                    }
                    result = true;
                }
                else
                {
                    _logger.Log($"P44 Log Check for {Region}:Region & Server:{Server}, Sending Email Alert with last 35 lines log text at TimeStamp {DateTime.UtcNow.ToString()} \n", FLLogger.eLogMessageType.eInfo);
                    EMailSubject = $": from P44 Service Log Check for {Region}:Region from Server:{Server}, generated at TimeStamp {DateTime.UtcNow.ToString()}";
                    eMail.SendEmailAlert(EMailSubject, $"P44LogFilePath {Region}:Region & Server:{Server} not found for the UTC Date {DateTime.UtcNow.ToString("yyyyMMdd")}");

                    //email.SendEmail(FromEmail, ToEmail, "EMail Alert:" + EMailSubject, $"P44LogFilePath {Region}:Region & Server:{Server} not found for the UTC Date {DateTime.UtcNow.ToString("yyyyMMdd")}" );
                }

            }
            catch (Exception ex)
            {
                
                _logger.Log("Process failed", FLLogger.eLogMessageType.eError);
                _logger.Log(ex.Message, FLLogger.eLogMessageType.eError);
                ErrorMsg = "Error Message: " + ex.Message;
                if (ex.InnerException != null)
                {
                    _logger.Log(ex.InnerException.ToString(), FLLogger.eLogMessageType.eError);
                    ErrorMsg = ErrorMsg + "\n Inner Exception: " + ex.Message;
                }

                _logger.Log(ex.StackTrace, FLLogger.eLogMessageType.eError);
                EMailSubject = $": from P44 Service Log Check for {Region}:Region from Server:{Server}, generated at TimeStamp {DateTime.UtcNow.ToString()}";
                //email.SendEmail(FromEmail, ToEmail, "EMail Alert:" + EMailSubject, ErrorMsg);
                eMail.SendErrorEMail(EMailSubject, ErrorMsg);

            }
            return result;
        }

    }
}
