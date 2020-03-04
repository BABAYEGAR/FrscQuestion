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
    public class RoleController : Controller
    {
        private readonly FrscQuestionDataContext _databaseConnection;

        public RoleController(FrscQuestionDataContext databaseConnection)
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
                !authorizedUser.Role.ManageRoles)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            return View(_databaseConnection.Roles.ToList());
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
                !authorizedUser.Role.ManageRoles)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            return View();
        }

        // POST: ImageCategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpireFilter]
        public ActionResult Create(Role role)
        {
            var authorizedUser = new AppUser();
            if (HttpContext.Session.GetString("FrscQuestionLoggedInUser") != null)
            {
                var userString = HttpContext.Session.GetString("FrscQuestionLoggedInUser");
                authorizedUser = JsonConvert.DeserializeObject<AppUser>(userString);
            }

            if (!authorizedUser.Role.AccessAdminConsole ||
                !authorizedUser.Role.ManageApplicationUser)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            try
            {
                var signedInUserId = Convert.ToInt64(HttpContext.Session.GetString("FrscQuestionLoggedInUserId"));
                role.DateCreated = DateTime.Now;
                role.DateLastModified = DateTime.Now;
                role.CreatedBy = signedInUserId;
                role.LastModifiedBy = signedInUserId;
                if (_databaseConnection.Roles.Where(n => n.Name == role.Name).ToList().Count > 0)
                {
                    //display notification
                    TempData["display"] = "Unable to perform the action because this record already exist!";
                    TempData["notificationtype"] = NotificationType.Error.ToString();
                    return View(role);
                }

                _databaseConnection.Roles.Add(role);
                _databaseConnection.SaveChanges();

                //display notification
                TempData["display"] = "You have successfully added a new Role!";
                TempData["notificationtype"] = NotificationType.Success.ToString();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //display notification
                TempData["display"] = ex.Message;
                TempData["notificationtype"] = NotificationType.Error.ToString();
                return View(role);
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
                !authorizedUser.Role.ManageRoles)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            return View(_databaseConnection.Roles.Find(id));
        }

        // POST: ImageCategory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpireFilter]
        public ActionResult Edit(Role role)
        {
            var authorizedUser = new AppUser();
            if (HttpContext.Session.GetString("FrscQuestionLoggedInUser") != null)
            {
                var userString = HttpContext.Session.GetString("FrscQuestionLoggedInUser");
                authorizedUser = JsonConvert.DeserializeObject<AppUser>(userString);
            }

            if (!authorizedUser.Role.AccessAdminConsole ||
                !authorizedUser.Role.ManageRoles)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            try
            {
                // TODO: Add update logic here
                var signedInUserId = Convert.ToInt64(HttpContext.Session.GetString("FrscQuestionLoggedInUserId"));
                role.DateLastModified = DateTime.Now;
                role.LastModifiedBy = signedInUserId;
                if (_databaseConnection.Roles.Where(n => n.Name == role.Name && n.RoleId != role.RoleId).ToList()
                        .Count > 0)
                {
                    //display notification
                    TempData["display"] = "Unable to perform the action because this record already exist!";
                    TempData["notificationtype"] = NotificationType.Error.ToString();
                    return View(role);
                }

                _databaseConnection.Entry(role).State = EntityState.Modified;
                _databaseConnection.SaveChanges();

                //display notification
                TempData["display"] = "You have successfully modified the Role!";
                TempData["notificationtype"] = NotificationType.Success.ToString();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //display notification
                TempData["display"] = ex.Message;
                TempData["notificationtype"] = NotificationType.Error.ToString();
                return View(role);
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
                !authorizedUser.Role.ManageRoles)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            var id = Convert.ToInt64(collection["Id"]);
            var role = _databaseConnection.Roles.Find(id);

            _databaseConnection.Roles.Remove(role);
            _databaseConnection.SaveChanges();

            //display notification
            TempData["display"] = "You have successfully deleted the Role!";
            TempData["notificationtype"] = NotificationType.Success.ToString();
            return RedirectToAction("Index");
        }
    }
}