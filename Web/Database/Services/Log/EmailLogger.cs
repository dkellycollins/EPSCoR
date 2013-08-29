using System;
using System.Net;
using System.Net.Mail;

namespace EPSCoR.Web.Database.Services.Log
{
    /// <summary>
    /// Sends an email when an error is logged.
    /// </summary>
    /// <remarks>Use cation with this logger. If site is unstable and sends multiple messages it could make the SMTP_HOST look like spam.</remarks>
    public class EmailLogger : ILogger
    {
        //TODO move this to a config file.
        private readonly static string[] RECIPIANTS = new string[] 
        {
            "dkellycollins@gmail.com"
        };
        private const string SMTP_HOST = "smtp.ksu.edu";

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
                mMessage.From = new MailAddress("server@daedulus.cis.ksu.edu");
                mMessage.Body = message + "\n" + e.Message + "\n" + e.StackTrace;

                using (SmtpClient smtp = new SmtpClient(SMTP_HOST))
                {
                    smtp.EnableSsl = true;
                    smtp.Credentials = new NetworkCredential("devinkc@ksu.edu", "TutTut1991", "");
                    try
                    {
                        smtp.Send(mMessage);
                    }
                    catch (Exception ex)
                    { }
                }
            }
        }
    }
}
