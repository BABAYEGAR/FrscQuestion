using System;
using System.Linq;
using FrscQuestion.Models.DataBaseConnections;
using FrscQuestion.Models.Encryption;
using FrscQuestion.Models.Entities;
using FrscQuestion.Models.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FrscQuestion.Controllers
{
    public class FaqController : Controller
    {
        private readonly FrscQuestionDataContext _databaseConnection;

        public FaqController(FrscQuestionDataContext databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        // GET: EmploymentPositions
        [SessionExpireFilter]
        public ActionResult Index()
        {
            var authorizedUser = new AppUser();
            if (HttpContext.Session.GetString("FrscQuestionLoggedInUser") != null)
            {
                var userString = HttpContext.Session.GetString("FrscQuestionLoggedInUser");
                authorizedUser = JsonConvert.DeserializeObject<AppUser>(userString);
            }

            if (!authorizedUser.Role.AccessAdminConsole ||
                !authorizedUser.Role.ManageFaq)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            return View(_databaseConnection.Faqs.ToList());
        }

        // GET: EmploymentPositions/Create
        [SessionExpireFilter]
        public ActionResult Create()
        {
            var authorizedUser = new AppUser();
            if (HttpContext.Session.GetString("FrscQuestionLoggedInUser") != null)
            {
                var userString = HttpContext.Session.GetString("FrscQuestionLoggedInUser");
                authorizedUser = JsonConvert.DeserializeObject<AppUser>(userString);
            }

            if (!authorizedUser.Role.AccessAdminConsole ||
                !authorizedUser.Role.ManageFaq)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpireFilter]
        public ActionResult Create(Faq faq)
        {
            var authorizedUser = new AppUser();
            if (HttpContext.Session.GetString("FrscQuestionLoggedInUser") != null)
            {
                var userString = HttpContext.Session.GetString("FrscQuestionLoggedInUser");
                authorizedUser = JsonConvert.DeserializeObject<AppUser>(userString);
            }

            if (!authorizedUser.Role.AccessAdminConsole ||
                !authorizedUser.Role.ManageFaq)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            try
            {
                var signedInUserId = Convert.ToInt64(HttpContext.Session.GetString("FrscQuestionLoggedInUserId"));
                faq.DateCreated = DateTime.Now;
                faq.DateLastModified = DateTime.Now;
                faq.CreatedBy = signedInUserId;
                faq.LastModifiedBy = signedInUserId;

                _databaseConnection.Faqs.Add(faq);
                _databaseConnection.SaveChanges();

                //display notification
                TempData["display"] = "You have successfully added a new FAQ!";
                TempData["notificationtype"] = NotificationType.Success.ToString();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //display notification
                TempData["display"] = ex.Message;
                TempData["notificationtype"] = NotificationType.Error.ToString();
                return View();
            }
        }

        // GET: EmploymentPositions/Edit/5
        [SessionExpireFilter]
        public ActionResult Edit(long id)
        {
            var authorizedUser = new AppUser();
            if (HttpContext.Session.GetString("FrscQuestionLoggedInUser") != null)
            {
                var userString = HttpContext.Session.GetString("FrscQuestionLoggedInUser");
                authorizedUser = JsonConvert.DeserializeObject<AppUser>(userString);
            }

            if (!authorizedUser.Role.AccessAdminConsole ||
                !authorizedUser.Role.ManageFaq)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            return View(_databaseConnection.Faqs.Find(id));
        }

        // POST: EmploymentPositions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpireFilter]
        public ActionResult Edit(Faq faq)
        {
            var authorizedUser = new AppUser();
            if (HttpContext.Session.GetString("FrscQuestionLoggedInUser") != null)
            {
                var userString = HttpContext.Session.GetString("FrscQuestionLoggedInUser");
                authorizedUser = JsonConvert.DeserializeObject<AppUser>(userString);
            }

            if (!authorizedUser.Role.AccessAdminConsole ||
                !authorizedUser.Role.ManageFaq)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            try
            {
                // TODO: Add update logic here
                var signedInUserId = Convert.ToInt64(HttpContext.Session.GetString("FrscQuestionLoggedInUserId"));
                faq.DateLastModified = DateTime.Now;
                faq.LastModifiedBy = signedInUserId;
                _databaseConnection.Entry(faq).State = EntityState.Modified;
                _databaseConnection.SaveChanges();

                //display notification
                TempData["display"] = "You have successfully modified the FAQ!";
                TempData["notificationtype"] = NotificationType.Success.ToString();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //display notification
                TempData["display"] = ex.Message;
                TempData["notificationtype"] = NotificationType.Error.ToString();
                return View();
            }
        }

        // GET: EmploymentPositions/Delete/5
        [SessionExpireFilter]
        public ActionResult Delete(IFormCollection collection)
        {
            var authorizedUser = new AppUser();
            if (HttpContext.Session.GetString("FrscQuestionLoggedInUser") != null)
            {
                var userString = HttpContext.Session.GetString("FrscQuestionLoggedInUser");
                authorizedUser = JsonConvert.DeserializeObject<AppUser>(userString);
            }

            if (!authorizedUser.Role.AccessAdminConsole ||
                !authorizedUser.Role.ManageFaq)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            var id = Convert.ToInt64(collection["Id"]);
            var faq = _databaseConnection.Faqs.Find(id);

            _databaseConnection.Faqs.Remove(faq);
            _databaseConnection.SaveChanges();

            //display notification
            TempData["display"] = "You have successfully deleted the FAQ!";
            TempData["notificationtype"] = NotificationType.Success.ToString();
            return RedirectToAction("Index");
        }
    }
}