using Footlocker;
using Footlocker.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P44ServiceLogEMailAlert
{

    public class EMail
    {
        //public string EMailSubject { get; set; }
        //public string EMailAlert { get; set; }
        public string ErrorMsg { get; set; }
        //string ErrorMsg = string.Empty;
        string FromEmail;
        string ToEmail;
        string ToCC;
        FLLogger _logger;
        EMailService email;
        public EMail(string Log)
        {
            _logger = new FLLogger(Log);
            FromEmail = ConfigurationManager.AppSettings["FromEmailGroup"].ToString();
            ToEmail = ConfigurationManager.AppSettings["ToEmailGroup"].ToString();
            ToCC = ConfigurationManager.AppSettings["ToCCEmailGroup"].ToString();
        }
        public void SendEmailAlert(string EMailSubject,string EMailBody)
        {
            try
            {
                email = new EMailService();
                //EMailSubject = $" P44 Service Log Check for {Region}:Region from Server:{Server}, generated at TimeStamp {DateTime.UtcNow.ToString()}";
                email.SendEmail(FromEmail, ToEmail, "Alert EMail: " + EMailSubject, EMailBody);
            }
            catch(Exception ex)
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
            }

        }
        public void SendErrorEMail(string EMailSubject, string EMailBody)
        {
            //"P44 Service, File Watcher E Mail Error !
            _logger.Log($" Calling process P44 Service, File Watcher E Mail Error !!!", FLLogger.eLogMessageType.eWarning);
            try
            {
                email = new EMailService();
                email.SendEmail(FromEmail, ToEmail, "Error EMail: " + EMailSubject, EMailBody);
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
            }
        }
    }
}
