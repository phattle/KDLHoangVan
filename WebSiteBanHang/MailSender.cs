using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace WebSiteBanHang
{
    public class MailSender
    {
        public string senderEmailAddress { get; set; }
        public string senderEmailPassword { get; set; }
        public string hostName { get; set; }
        public int port { get; set; } = 587;
        public MailSender(string senderEmailAddress, string senderEmailPassword, string hostName, int port)
        {
            this.senderEmailAddress = senderEmailAddress;
            this.senderEmailPassword = senderEmailPassword;
            this.hostName = hostName;
            this.port = port;
        }
        public Boolean GuiEmail(string Title, string _description, List< string> mailKH)
        {
            if (mailKH == null)
            {
                return false;
            }
            if (mailKH.Count <=0)
            {
                return false;
            }
            string senderID = senderEmailAddress;
            string senderPassword = senderEmailPassword;
            string body =  _description;
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(senderID);
                foreach (string item in mailKH)
                {
                    mail.To.Add(item);
                }
                mail.Subject = Title;
                mail.Body = body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = hostName; //Or Your SMTP Server Address
                smtp.Credentials = new System.Net.NetworkCredential(senderID, senderPassword);
                smtp.Port = port;// 587;
                smtp.EnableSsl = true;
                smtp.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
               // result = "problem occurred";

            }
            return false;
        }
    }
}