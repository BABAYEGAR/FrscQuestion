using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FrscQuestion.Models.Entities;
using MailKit.Net.Smtp;
using MimeKit;

namespace FrscQuestion.Models.Services
{
    public class Mailer
    {
        public void SendNewUserEmail(string path, AppUser appUser, AppUserAccessKey accessKey)
        {
            try
            {
                //From Address
                var fromAddress = "support@afriplugz.com";
                var fromAdressTitle = "Afriplugz";
                //To Address
                var toVendor = appUser.Email;
                //var toCustomer = email;
                var toAdressTitle = appUser.Name;
                var subject = "Afriplugz (Activate Account).";

                //Smtp Server
                var smtpServer = new AppConfig().EmailServer;
                //Smtp Port Number
                var smtpPortNumber = new AppConfig().Port;

                var mimeMessageVendor = new MimeMessage();
                mimeMessageVendor.From.Add(new MailboxAddress(fromAdressTitle, fromAddress));
                mimeMessageVendor.To.Add(new MailboxAddress(toAdressTitle, toVendor));
                mimeMessageVendor.Subject = subject;
                var bodyBuilder = new BodyBuilder();
                using (var data = File.OpenText(path))
                {
                    {
                        //manage content

                        bodyBuilder.HtmlBody = data.ReadToEnd();
                        var body = bodyBuilder.HtmlBody;

                        var replace = body.Replace("USER", appUser.Name);
                        replace = replace.Replace("URL",new AppConfig().AppUrl + "Account/AccountActivationLink?accessCode=" +
                                                        accessKey.AccountActivationAccessCode);
                        replace = replace.Replace("LOGO", new AppConfig().AppLogo);
                        replace = replace.Replace("APPURL", new AppConfig().AppUrl);
                        replace = replace.Replace("TC", new AppConfig().Terms);
                        replace = replace.Replace("PRIVACY", new AppConfig().PrivacyPolicy);
                        bodyBuilder.HtmlBody = replace;
                        mimeMessageVendor.Body = bodyBuilder.ToMessageBody();
                    }
                }

                using (var client = new SmtpClient())
                {
                    client.Connect(smtpServer, smtpPortNumber);
                    // Note: only needed if the SMTP server requires authentication
                    // Error 5.5.1 Authentication 
                    client.Authenticate(new AppConfig().SupportEmail, new AppConfig().SupportEmailPassword);
                    client.Send(mimeMessageVendor);
                    client.Disconnect(true);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
      public void SendNewUserSocialEmail(string path, AppUser appUser)
        {
            try
            {
                //From Address
                var fromAddress = "support@afriplugz.com";
                var fromAdressTitle = "Afriplugz";
                //To Address
                var toVendor = appUser.Email;
                //var toCustomer = email;
                var toAdressTitle = appUser.Name;
                var subject = "Welcome to Afriplugz";

                //Smtp Server
                var smtpServer = new AppConfig().EmailServer;
                //Smtp Port Number
                var smtpPortNumber = new AppConfig().Port;

                var mimeMessageVendor = new MimeMessage();
                mimeMessageVendor.From.Add(new MailboxAddress(fromAdressTitle, fromAddress));
                mimeMessageVendor.To.Add(new MailboxAddress(toAdressTitle, toVendor));
                mimeMessageVendor.Subject = subject;
                var bodyBuilder = new BodyBuilder();
                using (var data = File.OpenText(path))
                {
                    {
                        //manage content

                        bodyBuilder.HtmlBody = data.ReadToEnd();
                        var body = bodyBuilder.HtmlBody;

                        var replace = body.Replace("USER", appUser.Name);
                        replace = replace.Replace("APPURL", new AppConfig().AppUrl);
                        replace = replace.Replace("LOGO", new AppConfig().AppLogo);
                        replace = replace.Replace("TC", new AppConfig().Terms);
                        replace = replace.Replace("PRIVACY", new AppConfig().PrivacyPolicy);
                        bodyBuilder.HtmlBody = replace;
                        mimeMessageVendor.Body = bodyBuilder.ToMessageBody();
                    }
                }

                using (var client = new SmtpClient())
                {
                    client.Connect(smtpServer, smtpPortNumber);
                    // Note: only needed if the SMTP server requires authentication
                    // Error 5.5.1 Authentication 
                    client.Authenticate(new AppConfig().SupportEmail, new AppConfig().SupportEmailPassword);
                    client.Send(mimeMessageVendor);
                    client.Disconnect(true);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void SendForgotPasswordResetLink(string path, AppUser appUser, AppUserAccessKey accessKey)
        {
            //From Address
            var fromAddress = "support@gracelandinngarden.com";
            var fromAdressTitle = "Afriplugz";
            //To Address
            var toVendor = appUser.Email;
            //var toCustomer = email;
            var toAdressTitle = appUser.Name;
            var subject = "Afriplugz (Password Reset).";

            //Smtp Server
            var smtpServer = new AppConfig().EmailServer;
            //Smtp Port Number
            var smtpPortNumber = new AppConfig().Port;

            var mimeMessageVendor = new MimeMessage();
            mimeMessageVendor.From.Add(new MailboxAddress(fromAdressTitle, fromAddress));
            mimeMessageVendor.To.Add(new MailboxAddress(toAdressTitle, toVendor));
            mimeMessageVendor.Subject = subject;
            var bodyBuilder = new BodyBuilder();
            using (var data = File.OpenText(path))
            {
                {
                    //manage content

                    bodyBuilder.HtmlBody = data.ReadToEnd();
                    var body = bodyBuilder.HtmlBody;

                    var replace = body.Replace("USER", appUser.Name);
                    replace = replace.Replace("DATE", DateTime.Now.ToString(CultureInfo.InvariantCulture));

                    replace = replace.Replace("URL", new AppConfig().AppUrl +
                                                     "Account/ForgotPassword?accessCode=" +
                                                     accessKey.PasswordAccessCode);
                    replace = replace.Replace("LOGO", new AppConfig().AppLogo);
                    replace = replace.Replace("APPURL", new AppConfig().AppUrl);
                    replace = replace.Replace("TC", new AppConfig().Terms);
                    replace = replace.Replace("PRIVACY", new AppConfig().PrivacyPolicy);
                    bodyBuilder.HtmlBody = replace;
                    mimeMessageVendor.Body = bodyBuilder.ToMessageBody();
                }
            }

            using (var client = new SmtpClient())
            {
                client.Connect(smtpServer, smtpPortNumber, true);
                // Note: only needed if the SMTP server requires authentication
                // Error 5.5.1 Authentication 
                client.Authenticate(new AppConfig().SupportEmail, new AppConfig().SupportEmailPassword);
                client.Send(mimeMessageVendor);
                client.Disconnect(true);
            }
        }

        public void SendNewsletterEmail(string path, NewsLetter newsLetter, List<AppUser> appUsers)
        {
            try
            {
                var client = new SmtpClient();
                //Smtp Server
                var smtpServer = new AppConfig().EmailServer;
                //Smtp Port Number
                var smtpPortNumber = new AppConfig().Port;

                client.Connect(smtpServer, smtpPortNumber);
                // Note: only needed if the SMTP server requires authentication
                // Error 5.5.1 Authentication 
                client.Authenticate(new AppConfig().SupportEmail, new AppConfig().SupportEmailPassword);
                foreach (var item in appUsers)
                {
                    //From Address
                    var fromAddress = "info@afriplugz.com";
                    var fromAdressTitle = "Afriplugz Newsletter";
                    //To Address
                    var toVendor = item.Email;
                    //var toCustomer = email;
                    var toAdressTitle = item.Email;
                    var subject = newsLetter.Subject;
                    //var BodyContent = message;

                    var mimeMessageVendor = new MimeMessage();
                    mimeMessageVendor.From.Add(new MailboxAddress(fromAdressTitle, fromAddress));
                    var check = false;
                    try
                    {
                        mimeMessageVendor.To.Add(new MailboxAddress(toAdressTitle, toVendor));
                    }
                    catch (Exception e)
                    {
                        check = true;
                    }

                    if (check == false)
                    {
                        mimeMessageVendor.Subject = subject;
                        mimeMessageVendor.Priority = MessagePriority.Urgent;
                        var bodyBuilder = new BodyBuilder();
                        using (var data = File.OpenText(path))
                        {
                            {
                                //manage content

                                bodyBuilder.HtmlBody = data.ReadToEnd();
                                var body = bodyBuilder.HtmlBody;

                                var replace = body.Replace("URL", new AppConfig().AppUrl +
                                                                  "Home/UnSubscription?email=" + item.Email);
                                replace = replace.Replace("CONTENTBODY", newsLetter.Body);
                                replace = replace.Replace("USER", item.Name);
                                replace = replace.Replace("LOGO", new AppConfig().AppLogo);
                                replace = replace.Replace("APPURL", new AppConfig().AppUrl);
                                replace = replace.Replace("TC", new AppConfig().Terms);
                                replace = replace.Replace("PRIVACY", new AppConfig().PrivacyPolicy);
                                bodyBuilder.HtmlBody = replace;
                                mimeMessageVendor.Body = bodyBuilder.ToMessageBody();
                            }
                        }

                        client.Send(mimeMessageVendor);
                    }
                }

                client.Disconnect(false);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void SendGeneralNoticeEmail(string path, GeneralNotice generalNotice, List<AppUser> appUsers)
        {
            try
            {
                var client = new SmtpClient();
                //Smtp Server
                var smtpServer = new AppConfig().EmailServer;
                //Smtp Port Number
                var smtpPortNumber = new AppConfig().Port;

                client.Connect(smtpServer, smtpPortNumber);
                // Note: only needed if the SMTP server requires authentication
                // Error 5.5.1 Authentication 
                client.Authenticate(new AppConfig().SupportEmail, new AppConfig().SupportEmailPassword);
                foreach (var item in appUsers)
                {
                    //From Address
                    var fromAddress = "info@afriplugz.com";
                    var fromAdressTitle = "Afriplugz";
                    //To Address
                    var toVendor = item.Email;
                    //var toCustomer = email;
                    var toAdressTitle = item.Email;
                    var subject = generalNotice.Subject;
                    //var BodyContent = message;

                    var mimeMessageVendor = new MimeMessage();
                    mimeMessageVendor.From.Add(new MailboxAddress(fromAdressTitle, fromAddress));
                    var check = false;
                    try
                    {
                        mimeMessageVendor.To.Add(new MailboxAddress(toAdressTitle, toVendor));
                    }
                    catch (Exception)
                    {
                        check = true;
                    }

                    if (check == false)
                    {
                        mimeMessageVendor.Subject = subject;
                        mimeMessageVendor.Priority = MessagePriority.Urgent;
                        var bodyBuilder = new BodyBuilder();
                        using (var data = File.OpenText(path))
                        {
                            {
                                //manage content

                                bodyBuilder.HtmlBody = data.ReadToEnd();
                                var body = bodyBuilder.HtmlBody;

                                var replace = body.Replace("CONTENTBODY", generalNotice.Body);
                                replace = replace.Replace("USER", item.Name);
                                replace = replace.Replace("LOGO", new AppConfig().AppLogo);
                                replace = replace.Replace("APPURL", new AppConfig().AppUrl);
                                replace = replace.Replace("TC", new AppConfig().Terms);
                                replace = replace.Replace("PRIVACY", new AppConfig().PrivacyPolicy);
                                bodyBuilder.HtmlBody = replace;
                                mimeMessageVendor.Body = bodyBuilder.ToMessageBody();
                            }
                        }

                        client.Send(mimeMessageVendor);
                    }
                }

                client.Disconnect(false);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void SendContactEmail(string path, Contact contact)
        {
            //From Address
            var FromAddress = contact.Email;
            var FromAdressTitle = "Afriplugz Contact";
            //To Address
            var toVendor = "support@afriplugz.com";
            //var toCustomer = email;
            var ToAdressTitle = "Afriplugz";
            var Subject = "Afriplugz Contact";
            //var BodyContent = message;

            //Smtp Server
            var smtpServer = new AppConfig().EmailServer;
            //Smtp Port Number
            var smtpPortNumber = new AppConfig().Port;

            var mimeMessageVendor = new MimeMessage();
            mimeMessageVendor.From.Add(new MailboxAddress(FromAdressTitle, FromAddress));
            mimeMessageVendor.To.Add(new MailboxAddress(ToAdressTitle, toVendor));
            mimeMessageVendor.Subject = Subject;
            var bodyBuilder = new BodyBuilder();
            using (var data = File.OpenText(path))
            {
                {
                    //manage content

                    bodyBuilder.HtmlBody = data.ReadToEnd();
                    var body = bodyBuilder.HtmlBody;

                    var replace = body.Replace("USER", contact.Name);
                    replace = replace.Replace("NUMBER", contact.Mobile);
                    replace = replace.Replace("EMAIL", contact.Email);
                    replace = replace.Replace("DESCRIPTION", contact.Message);
                    replace = replace.Replace("SUBJECT", contact.Subject);
                    bodyBuilder.HtmlBody = replace;
                    mimeMessageVendor.Body = bodyBuilder.ToMessageBody();
                }
            }

            using (var client = new SmtpClient())
            {
                client.Connect(smtpServer, smtpPortNumber, true);
                // Note: only needed if the SMTP server requires authentication
                // Error 5.5.1 Authentication 
                client.Authenticate(new AppConfig().SupportEmail, new AppConfig().SupportEmailPassword);
                client.Send(mimeMessageVendor);
                client.Disconnect(true);
            }
        }
    }
}