using System;
using System.IO;
using System.Linq;
using FrscQuestion.Models;
using FrscQuestion.Models.DataBaseConnections;
using FrscQuestion.Models.Encryption;
using FrscQuestion.Models.Entities;
using FrscQuestion.Models.Enum;
using FrscQuestion.Models.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FrscQuestion.Controllers
{
    public class AppUserController : Controller
    {
        private readonly FrscQuestionDataContext _databaseConnection;
        private readonly IHostingEnvironment _hostingEnv;

        /// <summary>
        ///     Initialize some connections from the class constructor
        /// </summary>
        /// <param name="env"></param>
        /// <param name="databaseConnection"></param>
        public AppUserController(IHostingEnvironment env, FrscQuestionDataContext databaseConnection)
        {
            _databaseConnection = databaseConnection;
            _hostingEnv = env;
        }

        [SessionExpireFilter]
        public IActionResult Index()
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

            return View(_databaseConnection.AppUsers.Include(n => n.Role).ToList());
        }

        [SessionExpireFilter]
        public ActionResult ChangePassword(long id)
        {
            var model = new AccountModel {AppUserId = id};
            return View(model);
        }


        /// <summary>
        ///     Change User Password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpireFilter]
        public ActionResult ChangePassword(AccountModel model)
        {
            var access = new AccessLog();
            var signedInUserId = Convert.ToInt64(HttpContext.Session.GetString("FrscQuestionLoggedInUserId"));
            var appUser = _databaseConnection.AppUsers.Find(model.AppUserId);
            try
            {
                if (appUser != null)
                {
                    appUser.LastModifiedBy = signedInUserId;
                    appUser.DateLastModified = DateTime.Now;
                    appUser.Password = new Hashing().HashPassword(model.Password);
                    appUser.ConfirmPassword = appUser.Password;
                    _databaseConnection.Entry(appUser).State = EntityState.Modified;
                    _databaseConnection.SaveChanges();

                    //determine access logs save transaction
                    access.Message =
                        "You have successfully overridden the Account Password!";
                    access.Status = AccessStatus.Approved.ToString();
                    access.Category = AccessCategory.ForgotPassword.ToString();
                    access.DateCreated = DateTime.Now;
                    access.DateLastModified = DateTime.Now;
                    access.CreatedBy = appUser.AppUserId;
                    access.LastModifiedBy = appUser.AppUserId;
                    access.AppUserId = appUser.AppUserId;
                    _databaseConnection.AccessLogs.Add(access);
                    _databaseConnection.SaveChanges();
                }

                //display notification
                TempData["display"] = access.Message;
                TempData["notificationtype"] = NotificationType.Success.ToString();
                return RedirectToAction("Index", "AppUser");
            }
            catch (Exception)
            {
                //display notification
                TempData["display"] = "There was an issue overriding the account password, Check and Try again!";
                TempData["notificationtype"] = NotificationType.Error.ToString();
                return View("ChangePassword", model);
            }
        }

        [SessionExpireFilter]
        public IActionResult Create()
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

            ViewBag.RoleId = new SelectList(_databaseConnection.Roles.ToList(), "RoleId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpireFilter]
        public IActionResult Create(AppUser appUser, IFormFile ProfilePicture)
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
                appUser.CreatedBy = signedInUserId;
                appUser.LastModifiedBy = signedInUserId;
                appUser.DateCreated = DateTime.Now;
                appUser.DateLastModified = DateTime.Now;
                appUser.HasSocialMediaLogin = false;
                appUser.Status = UserStatus.Inactive.ToString();
                appUser.AccountType = LoginType.Platform.ToString();
                appUser.HasSocialMediaLogin = false;
                //generate password
                var password = new Md5Ecryption().RandomString(8);

                appUser.Password = new Hashing().HashPassword(password);
                appUser.ConfirmPassword = appUser.Password;

                if (_databaseConnection.AppUsers.Where(n => n.Email == appUser.Email).ToList().Count > 0)
                {
                    ViewBag.RoleId = new SelectList(_databaseConnection.Roles.ToList(), "RoleId", "Name",
                        appUser.RoleId);
                    TempData["display"] = "A user with the same email already exist!";
                    TempData["notificationtype"] = NotificationType.Error.ToString();
                    return View(appUser);
                }

                //upload user logo if any file is uploaded
                if (ProfilePicture != null && !string.IsNullOrEmpty(ProfilePicture.FileName))
                {
                    var fileInfo = new FileInfo(ProfilePicture.FileName);
                    var ext = fileInfo.Extension.ToLower();
                    var name = DateTime.Now.ToFileTime().ToString();
                    var fileName = name + ext;
                    var uploadedImage = _hostingEnv.WebRootPath + $@"\UploadedFiles\ProfilePicture\{fileName}";

                    using (var fs = System.IO.File.Create(uploadedImage))
                    {
                        if (fs != null)
                        {
                            ProfilePicture.CopyTo(fs);
                            fs.Flush();
                            appUser.ProfilePicture = fileName;
                        }
                    }
                }

                _databaseConnection.AppUsers.Add(appUser);
                _databaseConnection.SaveChanges();


                if (appUser.AppUserId > 0)
                {
                    //define acceskeys and save transactions
                    var accessKey = new AppUserAccessKey
                    {
                        PasswordAccessCode = new Md5Ecryption().RandomString(15),
                        AccountActivationAccessCode = new Md5Ecryption().RandomString(20),
                        CreatedBy = appUser.AppUserId,
                        LastModifiedBy = appUser.AppUserId,
                        DateCreated = DateTime.Now,
                        DateLastModified = DateTime.Now,
                        ExpiryDate = DateTime.Now.AddDays(1),
                        AppUserId = appUser.AppUserId
                    };
                    _databaseConnection.AppUserAccessKeys.Add(accessKey);
                    _databaseConnection.SaveChanges();
                    new Mailer().SendNewUserEmail(new AppConfig().NewUserHtml, appUser, accessKey);
                }

                TempData["display"] = "You have successfully added a new user!";
                TempData["notificationtype"] = NotificationType.Success.ToString();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.RoleId = new SelectList(_databaseConnection.Roles.ToList(), "RoleId", "Name", appUser.RoleId);
                //display notification
                TempData["display"] = ex.Message;
                TempData["notificationtype"] = NotificationType.Error.ToString();
                return View(appUser);
            }
        }

        [SessionExpireFilter]
        public IActionResult ActivateUser(long id)
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

            var appUser = _databaseConnection.AppUsers.Find(id);
            var signedInUserId = HttpContext.Session.GetInt32("FrscQuestionLoggedInUser");
            appUser.Status = UserStatus.Active.ToString();
            appUser.LastModifiedBy = signedInUserId;
            appUser.DateLastModified = DateTime.Now;
            _databaseConnection.Entry(appUser).State = EntityState.Modified;
            _databaseConnection.SaveChanges();
            TempData["display"] = "You have successfully activated the user!";
            TempData["notificationtype"] = NotificationType.Success.ToString();
            return RedirectToAction("Index");
        }

        [SessionExpireFilter]
        public IActionResult DeactivateUser(long id)
        {
            var appUser = _databaseConnection.AppUsers.Find(id);
            var signedInUserId = HttpContext.Session.GetInt32("FrscQuestionLoggedInUser");
            appUser.Status = UserStatus.Active.ToString();
            appUser.LastModifiedBy = signedInUserId;
            appUser.DateLastModified = DateTime.Now;
            _databaseConnection.Entry(appUser).State = EntityState.Modified;
            _databaseConnection.SaveChanges();
            TempData["display"] = "You have successfully deactivated the user!";
            TempData["notificationtype"] = NotificationType.Success.ToString();
            return RedirectToAction("Index");
        }

        [SessionExpireFilter]
        public IActionResult Edit(long id)
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

            var appUser = _databaseConnection.AppUsers.Find(id);
            ViewBag.RoleId = new SelectList(_databaseConnection.Roles.ToList(), "RoleId", "Name", appUser.RoleId);
            return View(appUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpireFilter]
        public IActionResult Edit(AppUser appUser, IFormFile ProfilePicture)
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
                var signedInUserId = HttpContext.Session.GetInt32("FrscQuestionLoggedInUser");
                appUser.LastModifiedBy = signedInUserId;
                appUser.DateLastModified = DateTime.Now;
                if (_databaseConnection.AppUsers
                        .Where(n => n.Email == appUser.Email && n.AppUserId != appUser.AppUserId).ToList().Count > 0)
                {
                    ViewBag.RoleId = new SelectList(_databaseConnection.Roles.ToList(), "RoleId", "Name",
                        appUser.RoleId);
                    TempData["display"] = "A user with the same email already exist!";
                    TempData["notificationtype"] = NotificationType.Error.ToString();
                    return View(appUser);
                }

                //upload user logo if any file is uploaded
                if (ProfilePicture != null && !string.IsNullOrEmpty(ProfilePicture.FileName))
                {
                    var fileInfo = new FileInfo(ProfilePicture.FileName);
                    var ext = fileInfo.Extension.ToLower();
                    var name = DateTime.Now.ToFileTime().ToString();
                    var fileName = name + ext;
                    var uploadedImage = _hostingEnv.WebRootPath + $@"\UploadedFiles\ProfilePicture\{fileName}";

                    using (var fs = System.IO.File.Create(uploadedImage))
                    {
                        if (fs != null)
                        {
                            ProfilePicture.CopyTo(fs);
                            fs.Flush();
                            appUser.ProfilePicture = fileName;
                        }
                    }
                }


                _databaseConnection.Entry(appUser).State = EntityState.Modified;
                _databaseConnection.SaveChanges();
                TempData["display"] = "You have successfully modified the user!";
                TempData["notificationtype"] = NotificationType.Success.ToString();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.RoleId = new SelectList(_databaseConnection.Roles.ToList(), "RoleId", "Name", appUser.RoleId);
                //display notification
                TempData["display"] = ex.Message;
                TempData["notificationtype"] = NotificationType.Error.ToString();
                return View(appUser);
            }
        }

        [SessionExpireFilter]
        public ActionResult Delete(long id)
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
                var appUser = _databaseConnection.AppUsers.Find(id);
                var accessKeys = _databaseConnection.AppUserAccessKeys.Where(n => n.AppUserId == id).ToList();
                var accessLogs = _databaseConnection.AccessLogs.Where(n => n.AppUserId == id).ToList();
                foreach (var accessLog in accessLogs)
                {
                    accessLog.AppUserId = null;
                    _databaseConnection.Entry(accessLog).State = EntityState.Modified;
                    _databaseConnection.SaveChanges();
                }

                foreach (var accessKey in accessKeys)
                {
                    accessKey.AppUserId = null;
                    _databaseConnection.Entry(accessKey).State = EntityState.Modified;
                    _databaseConnection.SaveChanges();
                }
                _databaseConnection.AppUsers.Remove(appUser);
                _databaseConnection.SaveChanges();

                TempData["display"] = "You have successfully deleted the user!";
                TempData["notificationtype"] = NotificationType.Success.ToString();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //display notification
                TempData["display"] = ex.Message;
                TempData["notificationtype"] = NotificationType.Error.ToString();
                return RedirectToAction("Index");
            }
        }
    }
}