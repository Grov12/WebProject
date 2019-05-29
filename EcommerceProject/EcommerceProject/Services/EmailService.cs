using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace EcommerceProject.Services
{
    public class EmailService
    {
        public void sendEmailAsync(string email)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("grovgrov1@gmail.com");
                mail.To.Add(email);
                mail.Subject = "Order";
                mail.Body = "Thanks for your purschase sir";
                mail.IsBodyHtml = true;
                using (System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient())
                {
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("grovgrov1@gmail.com", "Robinlol12345");
                    smtp.Send(mail);
                }
            }

        }
    }
}
