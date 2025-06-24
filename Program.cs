using Footlocker;
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
    
    class Program
    {
        
        static void Main(string[] args)
        {

            //P44LogEMailAlert objEMailAlert;
            FileWatcher fileWatcher;
            try
            {
                //Console.WriteLine("P44 Service, Log Information E Mail Alert Start !");
                //objEMailAlert = new P44LogEMailAlert();
                //bool result = objEMailAlert.GenerateEMailAlert();
                //if(result ==true)
                //    Console.WriteLine("P44 Service, Log Information E Mail Alert End Successfully !");
                //else
                //    Console.WriteLine("P44 Service, Log Information E Mail Alert End with Failure !!!");
                Console.WriteLine("Main Program");
                fileWatcher = new FileWatcher();
                fileWatcher.On_Start();
                Console.WriteLine("Press any key to exit !!!");
                Console.Read();
                
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
           
        }

        //private static void OnElapsedTime(object sender, ElapsedEventArgs e)
        //{
        //    WriteToFile("Service is recall at " + DateTime.Now);
        //    Console.WriteLine("Service is recall at " + DateTime.Now);
        //}
        //public static void WriteToFile(string Message)
        //{
        //    string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
        //    if (!Directory.Exists(path))
        //    {
        //        Directory.CreateDirectory(path);
        //    }
        //    string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
        //    if (!File.Exists(filepath))
        //    {
        //        // Create a file to write to.
        //        using (StreamWriter sw = File.CreateText(filepath))
        //        {
        //            sw.WriteLine(Message);
        //        }
        //    }
        //    else
        //    {
        //        using (StreamWriter sw = File.AppendText(filepath))
        //        {
        //            sw.WriteLine(Message);
        //        }
        //    }
        //}
    }
}
