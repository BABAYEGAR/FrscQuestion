using System;
using System.Collections.Generic;
using System.Linq;
using FrscQuestion.Models;
using FrscQuestion.Models.DataBaseConnections;
using FrscQuestion.Models.Encryption;
using FrscQuestion.Models.Entities;
using FrscQuestion.Models.Enum;
using FrscQuestion.Models.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FrscQuestion.Controllers
{
    public class PageController : Controller
    {
        private readonly FrscQuestionDataContext _databaseConnection;

        /// <summary>
        ///     Initialize some connections from the class constructor
        /// </summary>
        /// <param name="databaseConnection"></param>
        public PageController(FrscQuestionDataContext databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        public IActionResult AppCredential()
        {
            var authorizedUser = new AppUser();
            if (HttpContext.Session.GetString("FrscQuestionLoggedInUser") != null)
            {
                var userString = HttpContext.Session.GetString("FrscQuestionLoggedInUser");
                authorizedUser = JsonConvert.DeserializeObject<AppUser>(userString);
            }

            if (!authorizedUser.Role.AccessAdminConsole || !authorizedUser.Role.ManageApplicationUser)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            return View(_databaseConnection.AppCredentials.FirstOrDefault());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AppCredential(AppCredential credential)
        {
            var authorizedUser = new AppUser();
            if (HttpContext.Session.GetString("FrscQuestionLoggedInUser") != null)
            {
                var userString = HttpContext.Session.GetString("FrscQuestionLoggedInUser");
                authorizedUser = JsonConvert.DeserializeObject<AppUser>(userString);
            }

            if (!authorizedUser.Role.AccessAdminConsole || !authorizedUser.Role.ManageApplicationUser)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            var signedInUserId = Convert.ToInt64(HttpContext.Session.GetString("FrscQuestionLoggedInUserId"));
            credential.DateLastModified = DateTime.Now;
            credential.LastModifiedBy = signedInUserId;

            if (credential.AppCredentialId > 0)
            {
                credential.LastModifiedBy = signedInUserId;
                credential.DateLastModified = DateTime.Now;
                _databaseConnection.Entry(credential).State = EntityState.Modified;
                _databaseConnection.SaveChanges();
            }
            else
            {
                credential.CreatedBy = signedInUserId;
                credential.DateCreated = DateTime.Now;
                _databaseConnection.AppCredentials.Add(credential);
                _databaseConnection.SaveChanges();
            }

            TempData["display"] = "You have successfully modified the Application Private Credentials!";
            TempData["notificationtype"] = NotificationType.Success.ToString();
            return RedirectToAction("Dashboard", "User");
        }

        public IActionResult GeneralNotice()
        {
            var authorizedUser = new AppUser();
            if (HttpContext.Session.GetString("FrscQuestionLoggedInUser") != null)
            {
                var userString = HttpContext.Session.GetString("FrscQuestionLoggedInUser");
                authorizedUser = JsonConvert.DeserializeObject<AppUser>(userString);
            }

            if (!authorizedUser.Role.AccessAdminConsole || !authorizedUser.Role.ManageApplicationUser)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GeneralNotice(GeneralNotice generalNotice)
        {
            var authorizedUser = new AppUser();
            if (HttpContext.Session.GetString("FrscQuestionLoggedInUser") != null)
            {
                var userString = HttpContext.Session.GetString("FrscQuestionLoggedInUser");
                authorizedUser = JsonConvert.DeserializeObject<AppUser>(userString);
            }

            if (!authorizedUser.Role.AccessAdminConsole || !authorizedUser.Role.ManageApplicationUser)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            var signedInUserId = Convert.ToInt64(HttpContext.Session.GetString("FrscQuestionLoggedInUserId"));
            var users = _databaseConnection.AppUsers.ToList();

            generalNotice.DateCreated = DateTime.Now;
            generalNotice.DateLastModified = DateTime.Now;
            generalNotice.CreatedBy = signedInUserId;
            generalNotice.CreatedBy = signedInUserId;

            _databaseConnection.GeneralNotices.Add(generalNotice);
            _databaseConnection.SaveChanges();

            new Mailer().SendGeneralNoticeEmail(new AppConfig().GeneralNoticeHtml, generalNotice, users);
            TempData["display"] = "You have successfully sent a General Notice!";
            TempData["notificationtype"] = NotificationType.Success.ToString();
            return RedirectToAction("GeneralNotice", "Page");
        }

        public IActionResult NewsLetter()
        {
            var authorizedUser = new AppUser();
            if (HttpContext.Session.GetString("FrscQuestionLoggedInUser") != null)
            {
                var userString = HttpContext.Session.GetString("FrscQuestionLoggedInUser");
                authorizedUser = JsonConvert.DeserializeObject<AppUser>(userString);
            }

            if (!authorizedUser.Role.AccessAdminConsole || !authorizedUser.Role.ManageApplicationUser)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NewsLetter(NewsLetter newsLetter)
        {
            var authorizedUser = new AppUser();
            if (HttpContext.Session.GetString("FrscQuestionLoggedInUser") != null)
            {
                var userString = HttpContext.Session.GetString("FrscQuestionLoggedInUser");
                authorizedUser = JsonConvert.DeserializeObject<AppUser>(userString);
            }

            if (!authorizedUser.Role.AccessAdminConsole || !authorizedUser.Role.ManageApplicationUser)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            var users = new List<AppUser>();
            var signedInUserId = Convert.ToInt64(HttpContext.Session.GetString("FrscQuestionLoggedInUserId"));
            var subscriptions = _databaseConnection.Subscriptions.Where(n => n.Status == "Active").ToList();
            foreach (var item in subscriptions)
            {
                var user = new AppUser
                {
                    Name = item.Name,
                    Email = item.Email
                };
                users.Add(user);
            }

            newsLetter.DateCreated = DateTime.Now;
            newsLetter.DateLastModified = DateTime.Now;
            newsLetter.CreatedBy = signedInUserId;
            newsLetter.CreatedBy = signedInUserId;

            _databaseConnection.NewsLetters.Add(newsLetter);
            _databaseConnection.SaveChanges();
            new Mailer().SendNewsletterEmail(new AppConfig().NewsletterHtml, newsLetter, users);
            TempData["display"] = "You have successfully sent the Newsletter!";
            TempData["notificationtype"] = NotificationType.Success.ToString();
            return RedirectToAction("NewsLetter", "Page");
        }

        public IActionResult PrivacyPolicy()
        {
            var authorizedUser = new AppUser();
            if (HttpContext.Session.GetString("FrscQuestionLoggedInUser") != null)
            {
                var userString = HttpContext.Session.GetString("FrscQuestionLoggedInUser");
                authorizedUser = JsonConvert.DeserializeObject<AppUser>(userString);
            }

            if (!authorizedUser.Role.AccessAdminConsole || !authorizedUser.Role.ManageApplicationUser)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            return View(_databaseConnection.PrivacyPolicies.FirstOrDefault());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PrivacyPolicy(PrivacyPolicy policy)
        {
            var authorizedUser = new AppUser();
            if (HttpContext.Session.GetString("FrscQuestionLoggedInUser") != null)
            {
                var userString = HttpContext.Session.GetString("FrscQuestionLoggedInUser");
                authorizedUser = JsonConvert.DeserializeObject<AppUser>(userString);
            }

            if (!authorizedUser.Role.AccessAdminConsole || !authorizedUser.Role.ManageApplicationUser)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            var signedInUserId = Convert.ToInt64(HttpContext.Session.GetString("FrscQuestionLoggedInUserId"));
            policy.DateLastModified = DateTime.Now;
            policy.LastModifiedBy = signedInUserId;

            if (policy.PrivacyPolicyId > 0)
            {
                policy.LastModifiedBy = signedInUserId;
                policy.DateLastModified = DateTime.Now;
                _databaseConnection.Entry(policy).State = EntityState.Modified;
                _databaseConnection.SaveChanges();
            }
            else
            {
                policy.CreatedBy = signedInUserId;
                policy.DateCreated = DateTime.Now;
                _databaseConnection.PrivacyPolicies.Add(policy);
                _databaseConnection.SaveChanges();
            }

            TempData["display"] = "You have successfully modified the Privacy Policy!";
            TempData["notificationtype"] = NotificationType.Success.ToString();
            return RedirectToAction("Dashboard", "User");
        }

        public IActionResult Terms()
        {
            var authorizedUser = new AppUser();
            if (HttpContext.Session.GetString("FrscQuestionLoggedInUser") != null)
            {
                var userString = HttpContext.Session.GetString("FrscQuestionLoggedInUser");
                authorizedUser = JsonConvert.DeserializeObject<AppUser>(userString);
            }

            if (!authorizedUser.Role.AccessAdminConsole || !authorizedUser.Role.ManageApplicationUser)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            return View(_databaseConnection.TermsAndConditions.ToList());
        }

        public IActionResult CreateTerms()
        {
            var authorizedUser = new AppUser();
            if (HttpContext.Session.GetString("FrscQuestionLoggedInUser") != null)
            {
                var userString = HttpContext.Session.GetString("FrscQuestionLoggedInUser");
                authorizedUser = JsonConvert.DeserializeObject<AppUser>(userString);
            }

            if (!authorizedUser.Role.AccessAdminConsole || !authorizedUser.Role.ManageApplicationUser)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateTerms(TermAndCondition terms)
        {
            var authorizedUser = new AppUser();
            if (HttpContext.Session.GetString("FrscQuestionLoggedInUser") != null)
            {
                var userString = HttpContext.Session.GetString("FrscQuestionLoggedInUser");
                authorizedUser = JsonConvert.DeserializeObject<AppUser>(userString);
            }

            if (!authorizedUser.Role.AccessAdminConsole || !authorizedUser.Role.ManageApplicationUser)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            var signedInUserId = Convert.ToInt64(HttpContext.Session.GetString("FrscQuestionLoggedInUserId"));
            terms.DateLastModified = DateTime.Now;
            terms.DateCreated = DateTime.Now;
            terms.LastModifiedBy = signedInUserId;
            terms.CreatedBy = signedInUserId;

            _databaseConnection.TermsAndConditions.Add(terms);
            _databaseConnection.SaveChanges();
            TempData["display"] = "You have successfully added a new version of the T&C!";
            TempData["notificationtype"] = NotificationType.Success.ToString();
            return RedirectToAction("Terms");
        }

        public IActionResult EditTerms(long id)
        {
            var authorizedUser = new AppUser();
            if (HttpContext.Session.GetString("FrscQuestionLoggedInUser") != null)
            {
                var userString = HttpContext.Session.GetString("FrscQuestionLoggedInUser");
                authorizedUser = JsonConvert.DeserializeObject<AppUser>(userString);
            }

            if (!authorizedUser.Role.AccessAdminConsole || !authorizedUser.Role.ManageApplicationUser)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            return View(_databaseConnection.TermsAndConditions.Find(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditTerms(TermAndCondition terms)
        {
            var authorizedUser = new AppUser();
            if (HttpContext.Session.GetString("FrscQuestionLoggedInUser") != null)
            {
                var userString = HttpContext.Session.GetString("FrscQuestionLoggedInUser");
                authorizedUser = JsonConvert.DeserializeObject<AppUser>(userString);
            }

            if (!authorizedUser.Role.AccessAdminConsole || !authorizedUser.Role.ManageApplicationUser)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            var signedInUserId = Convert.ToInt64(HttpContext.Session.GetString("FrscQuestionLoggedInUserId"));
            var newterms = new TermAndCondition
            {
                Text = terms.Text,
                DateLastModified = DateTime.Now,
                LastModifiedBy = signedInUserId,
                DateCreated = DateTime.Now,
                CreatedBy = signedInUserId
            };

            _databaseConnection.TermsAndConditions.Add(newterms);
            _databaseConnection.SaveChanges();
            TempData["display"] = "You have successfully modified the version of the T&C!";
            TempData["notificationtype"] = NotificationType.Success.ToString();
            return RedirectToAction("Terms");
        }


        [SessionExpireFilter]
        public ActionResult DeleteTerms(IFormCollection collection)
        {
            var authorizedUser = new AppUser();
            if (HttpContext.Session.GetString("FrscQuestionLoggedInUser") != null)
            {
                var userString = HttpContext.Session.GetString("FrscQuestionLoggedInUser");
                authorizedUser = JsonConvert.DeserializeObject<AppUser>(userString);
            }

            if (!authorizedUser.Role.AccessAdminConsole || !authorizedUser.Role.ManageApplicationUser)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            var id = Convert.ToInt64(collection["Id"]);
            var term = _databaseConnection.TermsAndConditions.Find(id);

            _databaseConnection.TermsAndConditions.Remove(term);
            _databaseConnection.SaveChanges();

            //display notification
            TempData["display"] = "You have successfully deleted the T&C!";
            TempData["notificationtype"] = NotificationType.Success.ToString();
            return RedirectToAction("Terms");
        }
    }
}