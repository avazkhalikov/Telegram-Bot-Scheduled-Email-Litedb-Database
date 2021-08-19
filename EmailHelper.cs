using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Telegram.Bot.Echo
{
   public class EmailHelper
    {
        public static void WriteErrorLog(Exception ex)
        {
            try
            {
                string path = @"C:\Temp\LogFile.txt";
                using (StreamWriter sw = (File.Exists(path)) ? File.AppendText(path) : File.CreateText(path))
                {
                    sw.WriteLine(DateTime.Now.ToString() + ": " + ex.Source.ToString().Trim() + "; " + ex.Message.ToString().Trim());
                    sw.Flush();
                    sw.Close();
                }
            }
            catch
            {
            }
        }


        public static void WriteMessageLog(string Message)
        {            
            try
            {
                string path = @"C:\Temp\LogFile.txt";
                using (StreamWriter sw = (File.Exists(path)) ? File.AppendText(path) : File.CreateText(path))
                {                  
                    sw.WriteLine(DateTime.Now.ToString() + ": " + Message);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch
            {
            }
        }
        /// <summary>
        /// simple email sender.
        /// </summary>
        /// <param name="From"></param>
        /// <param name="To"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="ssl"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public static void SendEmail(string From, string To, string host, int port, bool ssl, string username, string password,
            string subject, string body)
        {
            try
            {
                using (var message = new MailMessage(From, To))
                {
                    message.Subject = subject;
                    message.Body = body;
                    using (SmtpClient client = new SmtpClient
                    {
                        EnableSsl = false,
                        Host = host,
                        Port = port,
                        Credentials = new NetworkCredential(username, password)
                    })
                    {
                        client.Send(message);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
            }
        }

        /// <summary>
        /// This helper class sends an email message using the System.Net.Mail namespace
        /// </summary>
        /// <param name="fromEmail">Sender email address</param>
        /// <param name="toEmail">Recipient email address</param>
        /// <param name="bcc">Blind carbon copy email address</param>
        /// <param name="cc">Carbon copy email address</param>
        /// <param name="subject">Subject of the email message</param>
        /// <param name="body">Body of the email message</param>
        /// <param name="attachment">File to attach</param>

        #region Static Members

        public static void SendMailMessage(string toEmail, string fromEmail, string bcc, string cc, string subject, string body, List<string> attachmentFullPath)
        {
            //create the MailMessage object
            MailMessage mMailMessage = new MailMessage();

            //set the sender address of the mail message
            if (!string.IsNullOrEmpty(fromEmail))
            {
                mMailMessage.From = new MailAddress(fromEmail);
            }

            //set the recipient address of the mail message
            mMailMessage.To.Add(new MailAddress(toEmail));

            //set the blind carbon copy address
            if (!string.IsNullOrEmpty(bcc))
            {
                mMailMessage.Bcc.Add(new MailAddress(bcc));
            }

            //set the carbon copy address
            if (!string.IsNullOrEmpty(cc))
            {
                mMailMessage.CC.Add(new MailAddress(cc));
            }

            //set the subject of the mail message
            if (!string.IsNullOrEmpty(subject))
            {
                mMailMessage.Subject = "UBH Web Application Notification";
            }
            else
            {
                mMailMessage.Subject = subject;
            }

            //set the body of the mail message
            mMailMessage.Body = body;

            //set the format of the mail message body
            mMailMessage.IsBodyHtml = false;

            //set the priority
            mMailMessage.Priority = MailPriority.Normal;

            //add any attachments from the filesystem
            foreach (var attachmentPath in attachmentFullPath)
            {
                Attachment mailAttachment = new Attachment(attachmentPath);
                mMailMessage.Attachments.Add(mailAttachment);
            }

            //create the SmtpClient instance
            SmtpClient mSmtpClient = new SmtpClient();

            //send the mail message
            mSmtpClient.Send(mMailMessage);
        }

        /// <summary>
        /// Determines whether an email address is valid.
        /// </summary>
        /// <param name="emailAddress">The email address to validate.</param>
        /// <returns>
        /// 	<c>true</c> if the email address is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidEmailAddress(string emailAddress)
        {
            // An empty or null string is not valid
            if (String.IsNullOrEmpty(emailAddress))
            {
                return (false);
            }

            // Regular expression to match valid email address
            string emailRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

            // Match the email address using a regular expression
            Regex re = new Regex(emailRegex);
            if (re.IsMatch(emailAddress))
                return (true);
            else
                return (false);
        }

        #endregion

    }
}
