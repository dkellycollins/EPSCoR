using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EPSCoR.Database.Services.Log
{
    public class EmailLogger : ILogger
    {
        //TODO move this to a config file.
        private readonly static string[] RECIPIANTS = new string[] 
        {
            "dkellycolllins@gmail.com"
        };
        private const string SMTP_HOST = "idk";

        public void Log(string message)
        {
            //Dont do anything for these messages.
        }

        public void Log(string message, Exception e)
        {
            using (MailMessage mMessage = new MailMessage())
            {
                foreach (string recipiant in RECIPIANTS)
                {
                    mMessage.To.Add(recipiant);
                }

                mMessage.Subject = "An error has occured with the server.";
                mMessage.From = new MailAddress("server@daedulus.beocat.cis.ksu.edu");
                mMessage.Body = message + "\n" + e.Message + "\n" + e.StackTrace;

                using (SmtpClient smtp = new SmtpClient(SMTP_HOST))
                {
                    smtp.Send(mMessage);
                }
            }
        }
    }
}
