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
    public class OffenseController : Controller
    {
        private readonly FrscQuestionDataContext _databaseConnection;

        public OffenseController(FrscQuestionDataContext databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        // GET: ItemCategory
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
                !authorizedUser.Role.ViewAnswer)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            return View(_databaseConnection.Answers.ToList());
        }

        // GET: ItemCategory/Create
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
                !authorizedUser.Role.AddOffense)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            return View();
        }

        // POST: ImageCategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpireFilter]
        public ActionResult Create(Offense offense)
        {
            var authorizedUser = new AppUser();
            if (HttpContext.Session.GetString("FrscQuestionLoggedInUser") != null)
            {
                var userString = HttpContext.Session.GetString("FrscQuestionLoggedInUser");
                authorizedUser = JsonConvert.DeserializeObject<AppUser>(userString);
            }

            if (!authorizedUser.Role.AccessAdminConsole ||
                !authorizedUser.Role.AddAnswer)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            try
            {
                var signedInUserId = Convert.ToInt64(HttpContext.Session.GetString("FrscQuestionLoggedInUserId"));
                offense.DateCreated = DateTime.Now;
                offense.DateLastModified = DateTime.Now;
                offense.CreatedBy = signedInUserId;
                offense.LastModifiedBy = signedInUserId;
                if (_databaseConnection.Offenses.Where(n => n.Name == offense.Name).ToList().Count > 0)
                {
                    //display notification
                    TempData["display"] = "Unable to perform the action because this record already exist!";
                    TempData["notificationtype"] = NotificationType.Error.ToString();
                    return View(offense);
                }

                _databaseConnection.Offenses.Add(offense);
                _databaseConnection.SaveChanges();

                //display notification
                TempData["display"] = "You have successfully added a new OOffense!";
                TempData["notificationtype"] = NotificationType.Success.ToString();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //display notification
                TempData["display"] = ex.Message;
                TempData["notificationtype"] = NotificationType.Error.ToString();
                return View(offense);
            }
        }

        // GET: ImageCategory/Edit/5
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
                !authorizedUser.Role.EditOffense)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            return View(_databaseConnection.Offenses.Find(id));
        }

        // POST: ImageCategory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpireFilter]
        public ActionResult Edit(Offense offense)
        {
            var authorizedUser = new AppUser();
            if (HttpContext.Session.GetString("FrscQuestionLoggedInUser") != null)
            {
                var userString = HttpContext.Session.GetString("FrscQuestionLoggedInUser");
                authorizedUser = JsonConvert.DeserializeObject<AppUser>(userString);
            }

            if (!authorizedUser.Role.AccessAdminConsole ||
                !authorizedUser.Role.EditOffense)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            try
            {
                // TODO: Add update logic here
                var signedInUserId = Convert.ToInt64(HttpContext.Session.GetString("FrscQuestionLoggedInUserId"));
                offense.DateLastModified = DateTime.Now;
                offense.LastModifiedBy = signedInUserId;
                if (_databaseConnection.Offenses.Where(n => n.Name == offense.Name && n.OffenseId != offense.OffenseId).ToList()
                        .Count > 0)
                {
                    //display notification
                    TempData["display"] = "Unable to perform the action because this record already exist!";
                    TempData["notificationtype"] = NotificationType.Error.ToString();
                    return View(offense);
                }

                _databaseConnection.Entry(offense).State = EntityState.Modified;
                _databaseConnection.SaveChanges();

                //display notification
                TempData["display"] = "You have successfully modified the Offense!";
                TempData["notificationtype"] = NotificationType.Success.ToString();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //display notification
                TempData["display"] = ex.Message;
                TempData["notificationtype"] = NotificationType.Error.ToString();
                return View(offense);
            }
        }

        // GET: ImageCategory/Delete/5
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
                !authorizedUser.Role.DeleteOffense)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            var id = Convert.ToInt64(collection["Id"]);
            var offense = _databaseConnection.Offenses.Find(id);

            _databaseConnection.Offenses.Remove(offense);
            _databaseConnection.SaveChanges();

            //display notification
            TempData["display"] = "You have successfully deleted the Offense!";
            TempData["notificationtype"] = NotificationType.Success.ToString();
            return RedirectToAction("Index");
        }
    }
}