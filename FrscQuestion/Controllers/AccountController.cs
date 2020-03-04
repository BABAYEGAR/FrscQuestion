using System;
using System.Collections.Generic;
using System.Linq;
using FrscQuestion.Models;
using FrscQuestion.Models.DataBaseConnections;
using FrscQuestion.Models.Encryption;
using FrscQuestion.Models.Entities;
using FrscQuestion.Models.Enum;
using FrscQuestion.Models.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FrscQuestion.Controllers
{
    public class AccountController : Controller
    {
        private readonly FrscQuestionDataContext _databaseConnection;

        public AccountController(FrscQuestionDataContext databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }


        /// <summary>
        ///     Activate user account from SSO
        /// </summary>
        /// <param name="accessCode"></param>
        /// <returns></returns>
        public ActionResult AccountActivationLink(string accessCode)
        {
            var accessKey =
                _databaseConnection.AppUserAccessKeys.SingleOrDefault(n => n.AccountActivationAccessCode == accessCode);
            var appUser =
                _databaseConnection.AppUsers.Include(n => n.Role)
                    .SingleOrDefault(n =>
                        accessKey != null && n.AppUserId == accessKey.AppUserId);
            if (appUser != null)
            {
                if (appUser.Status == UserStatus.Inactive.ToString())
                {
                    //update user
                    appUser.Status = UserStatus.Active.ToString();
                    _databaseConnection.Entry(appUser).State = EntityState.Modified;
                    _databaseConnection.SaveChanges();
                    if (accessKey != null)
                    {
                        //update accessKeys
                        accessKey.AccountActivationAccessCode = new Md5Ecryption().RandomString(24);
                        accessKey.DateLastModified = DateTime.Now;
                        accessKey.ExpiryDate = DateTime.Now.AddDays(1);
                        _databaseConnection.Entry(accessKey).State = EntityState.Modified;
                        _databaseConnection.SaveChanges();

                        HttpContext.Session.SetString("FrscQuestionLoggedInUserId", appUser.AppUserId.ToString());
                        HttpContext.Session.SetString("FrscQuestionLoggedInUser", JsonConvert.SerializeObject(appUser));
                        //display notification
                        TempData["display"] =
                            "You have successfully verified your account!";
                        TempData["notificationtype"] = NotificationType.Success.ToString();
                        return RedirectToAction("Index", "Home");
                    }

                    //display notification
                    TempData["display"] =
                        "There was an issue Activating your Account Try again or Contact Graceland Support!";
                    TempData["notificationtype"] = NotificationType.Error.ToString();
                }

                if (appUser.Status == UserStatus.Active.ToString())
                {
                    //display notification
                    TempData["display"] =
                        "You have already activated your account, use your username and password to login!";
                    TempData["notificationtype"] = NotificationType.Info.ToString();
                    return RedirectToAction("Index", "Home");
                }
            }

            //display notification
            TempData["display"] =
                "Your Request is Invalid, Try again Later!";
            TempData["notificationtype"] = NotificationType.Error.ToString();
            return RedirectToAction("Login", "Account");
        }
        [SessionExpireFilter]
        public ActionResult ManageProfile()
        {
            try
            {
                var userString = HttpContext.Session.GetString("FrscQuestionLoggedInUser");
                var appUser = JsonConvert.DeserializeObject<AppUser>(userString);
                return View(appUser);
            }
            catch (Exception)
            {
                //display notification
                TempData["display"] = "Request is unavailable, Try again Later!";
                TempData["notificationtype"] = NotificationType.Error.ToString();
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        ///     Edit User Profile
        /// </summary>
        /// <param name="appUser"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpireFilter]
        public ActionResult EditProfile(AppUser appUser)
        {
            var access = new AccessLog();
            try
            {
                //populate object and save transaction
                var signedInUserId = Convert.ToInt64(HttpContext.Session.GetString("FrscQuestionLoggedInUserId"));
                appUser.LastModifiedBy = signedInUserId;
                appUser.DateLastModified = DateTime.Now;
                _databaseConnection.Entry(appUser).State = EntityState.Modified;
                _databaseConnection.SaveChanges();

                var userString = JsonConvert.SerializeObject(_databaseConnection.AppUsers
                    .Include(n => n.Role).SingleOrDefault(n => n.AppUserId == appUser.AppUserId));
                HttpContext.Session.SetString("FrscQuestionLoggedInUser", userString);

                access.Message = "You have successfully updated your profile!";
                access.Status = AccessStatus.Approved.ToString();
                access.Category = AccessCategory.ProfileUpdate.ToString();
                access.DateCreated = DateTime.Now;
                access.DateLastModified = DateTime.Now;
                _databaseConnection.AccessLogs.Add(access);
                _databaseConnection.SaveChanges();

                //display notification
                TempData["display"] = access.Message;
                TempData["notificationtype"] = NotificationType.Success.ToString();
                return RedirectToAction("ManageProfile");
            }
            catch (Exception)
            {
                //display notification
                TempData["display"] = "You are unable to update your profile, Check and Try again!";
                TempData["notificationtype"] = NotificationType.Error.ToString();
                return View("ManageProfile", appUser);
            }
        }

        /// <summary>
        ///     Change User Password
        /// </summary>
        /// <param name="appUser"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpireFilter]
        public ActionResult ChangePassword(AppUser appUser)
        {
            var access = new AccessLog();
            var signedInUserId = Convert.ToInt64(HttpContext.Session.GetString("FrscQuestionLoggedInUserId"));
            try
            {
                if (appUser != null)
                {
                    appUser.LastModifiedBy = signedInUserId;
                    appUser.DateLastModified = DateTime.Now;
                    appUser.Password = new Hashing().HashPassword(appUser.Password);
                    appUser.ConfirmPassword = appUser.Password;
                    _databaseConnection.Entry(appUser).State = EntityState.Modified;
                    _databaseConnection.SaveChanges();

                    //determine access logs save transaction
                    access.Message =
                        "You have successfully changed your Account Password!";
                    access.Status = AccessStatus.Approved.ToString();
                    access.Category = AccessCategory.ForgotPassword.ToString();
                    access.DateCreated = DateTime.Now;
                    access.DateLastModified = DateTime.Now;
                    access.CreatedBy = appUser.AppUserId;
                    access.LastModifiedBy = appUser.AppUserId;
                    access.AppUserId = appUser.AppUserId;
                    _databaseConnection.AccessLogs.Add(access);
                    _databaseConnection.SaveChanges();

                    var newUserString = JsonConvert.SerializeObject(appUser);
                    HttpContext.Session.SetString("FrscQuestionLoggedInUser", newUserString);
                }

                //display notification
                TempData["display"] = access.Message;
                TempData["notificationtype"] = NotificationType.Success.ToString();
                return RedirectToAction("ManageProfile");
            }
            catch (Exception)
            {
                //display notification
                TempData["display"] = "There was an issue changing your password, Check and Try again!";
                TempData["notificationtype"] = NotificationType.Error.ToString();
                return View("ManageProfile", appUser);
            }
        }

        public ActionResult ForgotPasswordLink()
        {
            return View();
        }

        public ActionResult ForgotPassword(string accessCode)
        {
            var model = new AccountModel();
            var accessKey =
                _databaseConnection.AppUserAccessKeys.SingleOrDefault(n => n.PasswordAccessCode == accessCode);
            if (accessKey != null)
            {
                if (DateTime.Now > accessKey.ExpiryDate)
                {
                    //display notification
                    TempData["display"] = "This link has already expired, Reset the password again!";
                    TempData["notificationtype"] = NotificationType.Error.ToString();
                    return RedirectToAction("Login", "Account");
                }

                var user = _databaseConnection.AppUsers.SingleOrDefault(n => n.AppUserId == accessKey.AppUserId);
                if (user != null)
                {
                    model.Email = user.Email;
                    model.LoginName = user.Email;
                }

                //update accessKeys
                if (user != null)
                {
                    //update accessKeys
                    accessKey.PasswordAccessCode = new Md5Ecryption().RandomString(24);
                    accessKey.DateLastModified = DateTime.Now;
                    accessKey.ExpiryDate = DateTime.Now.AddDays(1);
                    _databaseConnection.Entry(accessKey).State = EntityState.Modified;
                    _databaseConnection.SaveChanges();
                }

                return View(model);
            }

            //display notification
            TempData["display"] = "This link is not genuine!";
            TempData["notificationtype"] = NotificationType.Error.ToString();
            return View(model);
        }


        /// <summary>
        ///     validate and Password reset link
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPasswordLink(AccountModel model)
        {
            var appUser = _databaseConnection.AppUsers.SingleOrDefault(n => n.Email == model.Email);
            var accessKey =
                _databaseConnection.AppUserAccessKeys.SingleOrDefault(n => n.AppUserId == appUser.AppUserId);
            new Mailer().SendForgotPasswordResetLink(new AppConfig().ForgotPasswordHtml, appUser, accessKey);

            //display notification
            TempData["display"] =
                "The Account has been successfully Verified and the reset password link has been sent to your email";
            TempData["notificationtype"] = NotificationType.Success.ToString();
            return View("Login", model);
        }


        /// <summary>
        ///     Reset Password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(AccountModel model)
        {
            var access = new AccessLog();
            //populate object and save transaction
            var email = model.LoginName.ToLower();
            var user = _databaseConnection.AppUsers.SingleOrDefault(n => n.Email.ToLower() == email);
            try
            {
                //invalid user because the user username exists
                if (user == null)
                {
                    access.Message = "The Account doesn't exist!";
                    access.Status = AccessStatus.Denied.ToString();
                    access.Category = AccessCategory.ForgotPassword.ToString();
                    access.DateCreated = DateTime.Now;
                    access.DateLastModified = DateTime.Now;
                    _databaseConnection.AccessLogs.Add(access);
                    _databaseConnection.SaveChanges();
                    model = null;
                }
                //valid user
                else
                {
                    user.Password = new Hashing().HashPassword(model.Password);
                    user.ConfirmPassword = user.Password;
                    _databaseConnection.Entry(user).State = EntityState.Modified;
                    _databaseConnection.SaveChanges();

                    //determine access logs save transaction
                    access.Message =
                        "You have successfully reset your Account Password!";
                    access.Status = AccessStatus.Approved.ToString();
                    access.Category = AccessCategory.ForgotPassword.ToString();
                    access.DateCreated = DateTime.Now;
                    access.DateLastModified = DateTime.Now;
                    access.CreatedBy = user.AppUserId;
                    access.LastModifiedBy = user.AppUserId;
                    access.AppUserId = user.AppUserId;
                    _databaseConnection.AccessLogs.Add(access);
                    _databaseConnection.SaveChanges();
                }

                //display notification
                TempData["display"] = access.Message;
                TempData["notificationtype"] = NotificationType.Success.ToString();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                //display notification
                TempData["display"] = "You are unable to Reset your Password, Check and Try again!";
                TempData["notificationtype"] = NotificationType.Error.ToString();
                return View("ForgotPassword", model);
            }
        }

        public ActionResult Register()
        {
            var authorizedUser = new AppUser();
            if (HttpContext.Session.GetString("FrscQuestionLoggedInUser") != null)
            {
                var userString = HttpContext.Session.GetString("FrscQuestionLoggedInUser");
                authorizedUser = JsonConvert.DeserializeObject<AppUser>(userString);
            }

            var returnValue = HttpContext.Session.GetString("CheckOutRegister");

            if (authorizedUser != null && authorizedUser.Role != null && authorizedUser.AppUserId > 0)
            {
                return RedirectToAction("Index", "Home");
            }

            if (returnValue != null && returnValue == "true")
            {
                return View();
            }

            _databaseConnection.Dispose();
            HttpContext.Session.Clear();
            return View();
        }

        /// <summary>
        ///     Register new User to SSO
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Register(AccountModel model)
        {
            var access = new AccessLog();
            var email = model.Email.ToLower();
            var userExist = _databaseConnection.AppUsers.Include(n => n.Role).SingleOrDefault(
                n => n.Email.ToLower() == email);
            var hashPassword = new Hashing().HashPassword(model.Password);
            try
            {
                var appUser = new AppUser
                {
                    Name = model.LoginName,
                    Mobile = model.Mobile,
                    Email = model.Email,
                    Status = UserStatus.Inactive.ToString(),
                    ProfilePicture =
                        "../images/avatar.png",
                    DateCreated = DateTime.Now,
                    DateLastModified = DateTime.Now,
                    RoleId = _databaseConnection.AppCredentials.FirstOrDefault()?.EventPlannerId,
                    Password = hashPassword,
                    ConfirmPassword = hashPassword,
                    Address = "N/A",
                    AccountType = LoginType.Platform.ToString(),
                    HasSocialMediaLogin = false
                };

                //invalid user because the user email exists
                if (userExist != null)
                {
                    access.Message = "A user with the same Email already exist, try another Credential!";
                    access.Status = AccessStatus.Denied.ToString();
                    access.Category = AccessCategory.Registration.ToString();
                    access.DateCreated = DateTime.Now;
                    access.DateLastModified = DateTime.Now;
                    _databaseConnection.AccessLogs.Add(access);
                    _databaseConnection.SaveChanges();
                    //display notification
                    TempData["display"] = access.Message;
                    TempData["notificationtype"] = NotificationType.Error.ToString();
                    return View("Register", model);
                }
                //valid user

                _databaseConnection.AppUsers.Add(appUser);
                _databaseConnection.SaveChanges();
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

                //determine access logs save transaction
                access.Message =
                    "You have successfully registered, Check your email to confirm your account!";
                access.Status = AccessStatus.Approved.ToString();
                access.Category = AccessCategory.Registration.ToString();
                access.DateCreated = DateTime.Now;
                access.DateLastModified = DateTime.Now;
                access.AppUserId = appUser.AppUserId;
                _databaseConnection.AccessLogs.Add(access);
                _databaseConnection.SaveChanges();


                if (_databaseConnection.Subscriptions.Where(n =>
                            n.Email == appUser.Email).ToList()
                        .Count <= 0)
                {
                    if (appUser.AppUserId > 0)
                    {
                        var subscription = new Subscription
                        {
                            Email = appUser.Email,
                            Name = appUser.Name,
                            Status = "Active",
                            DateCreated = DateTime.Now,
                            DateLastModified = DateTime.Now,
                            CreatedBy = appUser.AppUserId,
                            LastModifiedBy = appUser.AppUserId
                        };
                        _databaseConnection.Add(subscription);
                    }

                    _databaseConnection.SaveChanges();
                }
                //create and populate user transport object
                new Mailer().SendNewUserEmail(new AppConfig().NewUserHtml, appUser, accessKey);


                //display notification
                if (appUser.AppUserId > 0)
                {
                    TempData["display"] =
                        access.Message;
                    TempData["notificationtype"] = NotificationType.Success.ToString();
                }

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                //display notification
                TempData["display"] = ex.ToString();
                TempData["notificationtype"] = NotificationType.Error.ToString();
                return View("Register", model);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            var authorizedUser = new AppUser();
            if (HttpContext.Session.GetString("FrscQuestionLoggedInUser") != null)
            {
                var userString = HttpContext.Session.GetString("FrscQuestionLoggedInUser");
                authorizedUser = JsonConvert.DeserializeObject<AppUser>(userString);
            }

            if (returnUrl != null && returnUrl == "checkout")
            {
                HttpContext.Session.SetString("CheckOutRegister", "true");
                return View();
            }

            if (returnUrl != null && returnUrl == "sessionExpired")
            {
                _databaseConnection.Dispose();
                HttpContext.Session.Clear();

                //display notification
                TempData["display"] = "Your session has expired, Login to continue!";
                TempData["notificationtype"] = NotificationType.Info.ToString();
            }

            if (returnUrl != null && returnUrl == "logOut")
            {
                _databaseConnection.Dispose();
                HttpContext.Session.Clear();

                //display notification
                TempData["display"] = "You have successfully Logged out!";
                TempData["notificationtype"] = NotificationType.Success.ToString();
            }

            if (authorizedUser != null && authorizedUser.Role != null && authorizedUser.AppUserId > 0)
            {
                return RedirectToAction("Index", "Home");
            }

            _databaseConnection.Dispose();
            HttpContext.Session.Clear();
            return View();
        }

        /// <summary>
        ///     Login User
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
          [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(AccountModel model)
        {
            var access = new AccessLog();
            var email = model.Email.ToLower();
            var userExist = _databaseConnection.AppUsers
                .Include(n => n.Role).SingleOrDefault(
                    n => n.Email.ToLower() == email);
            try
            {
                if (model.LoginType != LoginType.Google.ToString() &&
                    model.LoginType != LoginType.Facebook.ToString() &&
                    model.LoginType != LoginType.Twitter.ToString())
                {
                    model.LoginType = LoginType.Platform.ToString();

                    //for platform login
                    if (model.LoginType == LoginType.Platform.ToString())
                    {
                        if (userExist == null)
                        {
                            access.Message = "Your Email/Password is Incorrect. Try again!";
                            access.Status = AccessStatus.Denied.ToString();
                            access.Category = AccessCategory.Login.ToString();
                            access.DateCreated = DateTime.Now;
                            access.DateLastModified = DateTime.Now;
                            access.AppUser = null;
                            _databaseConnection.AccessLogs.Add(access);
                            _databaseConnection.SaveChanges();
                        }
                        else
                        {
                            if (userExist.HasSocialMediaLogin == false)
                            {
                                if (userExist.Status == UserStatus.Inactive.ToString())
                                {
                                    access.Message =
                                        "You are yet to activate your account from the the link sent to your email when you created the account!";
                                    access.Status = AccessStatus.Denied.ToString();
                                    access.Category = AccessCategory.Login.ToString();
                                    access.DateCreated = DateTime.Now;
                                    access.DateLastModified = DateTime.Now;
                                    access.CreatedBy = userExist.AppUserId;
                                    access.LastModifiedBy = userExist.AppUserId;
                                    access.AppUser = null;
                                    _databaseConnection.AccessLogs.Add(access);
                                    _databaseConnection.SaveChanges();
                                    userExist = null;
                                }

                                var passwordCorrect = userExist != null &&
                                                      new Hashing().ValidatePassword(model.Password,
                                                          userExist.ConfirmPassword);
                                if (passwordCorrect == false)
                                    if (userExist != null)
                                    {
                                        access.Message = "Your Email/Password is Incorrect. Try again!";
                                        access.Status = AccessStatus.Denied.ToString();
                                        access.Category = AccessCategory.Login.ToString();
                                        access.DateCreated = DateTime.Now;
                                        access.DateLastModified = DateTime.Now;
                                        access.CreatedBy = userExist.AppUserId;
                                        access.AppUser = null;
                                        access.LastModifiedBy = userExist.AppUserId;
                                        _databaseConnection.AccessLogs.Add(access);
                                        _databaseConnection.SaveChanges();
                                        userExist = null;
                                    }

                                if (passwordCorrect)
                                {
                                    access.Message = "Dear " + userExist.Name + ", You have successfully logged in!";
                                    access.Status = AccessStatus.Approved.ToString();
                                    access.Category = AccessCategory.Login.ToString();
                                    access.DateCreated = DateTime.Now;
                                    access.AppUserId = userExist.AppUserId;
                                    access.DateLastModified = DateTime.Now;
                                    access.CreatedBy = userExist.AppUserId;
                                    access.LastModifiedBy = userExist.AppUserId;

                                    _databaseConnection.AccessLogs.Add(access);
                                    _databaseConnection.SaveChanges();
                                }
                            }
                            else
                            {
                                access.Message =
                                    "Your Email/Password is Incorrect. Try again!";
                                access.Status = AccessStatus.Denied.ToString();
                                access.Category = AccessCategory.Login.ToString();
                                access.DateCreated = DateTime.Now;
                                access.DateLastModified = DateTime.Now;
                                access.CreatedBy = null;
                                access.LastModifiedBy = null;
                                access.AppUser = null;
                                _databaseConnection.AccessLogs.Add(access);
                                _databaseConnection.SaveChanges();
                                userExist = null;
                            }
                        }
                    }
                }

                //for social media login
                if (model.LoginType != LoginType.Platform.ToString())
                {
                    if (userExist == null)
                    {
                        var appUser = new AppUser
                        {
                            Name = model.UserName,
                            Mobile = "N/A",
                            Email = model.Email,
                            Status = UserStatus.Active.ToString(),
                            Address = "N/A",
                            ProfilePicture = model.ProfilePicture,
                            RoleId = _databaseConnection.AppCredentials.FirstOrDefault()?.CustomerId,
                            DateCreated = DateTime.Now,
                            DateLastModified = DateTime.Now,
                            CreatedBy = null,
                            LastModifiedBy = null,
                            HasSocialMediaLogin = true,
                            AccountType = model.LoginType
                        };
                        if (string.IsNullOrEmpty(appUser.Password))
                        {
                            appUser.Password = new Hashing().HashPassword(new Md5Ecryption().RandomString(5));
                            appUser.ConfirmPassword = appUser.Password;
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
                            access.Message = "You have successfully logged in!";
                            access.Status = AccessStatus.Approved.ToString();
                            access.Category = AccessCategory.Login.ToString();
                            access.DateCreated = DateTime.Now;
                            access.DateLastModified = DateTime.Now;
                            access.AppUser = null;
                            _databaseConnection.AccessLogs.Add(access);
                            _databaseConnection.SaveChanges();
                            //create and populate user transport object
                            new Mailer().SendNewUserSocialEmail(new AppConfig().NewUserSocialHtml, appUser);
                            userExist = _databaseConnection.AppUsers
                                .Include(n=>n.Role).SingleOrDefault(n=>n.AppUserId == appUser.AppUserId);
                        }
                    }
                    else
                    {
                        userExist.ProfilePicture = model.ProfilePicture;
                        userExist.DateLastModified = DateTime.Now;
                        //update user
                        _databaseConnection.Entry(userExist).State = EntityState.Modified;
                        _databaseConnection.SaveChanges();

                        access.Message = "Dear " + userExist.Name + " You have successfully logged in!";
                        access.Status = AccessStatus.Approved.ToString();
                        access.Category = AccessCategory.Login.ToString();
                        access.DateCreated = DateTime.Now;
                        access.DateLastModified = DateTime.Now;
                        access.AppUser = null;
                        access.AppUserId = userExist.AppUserId;
                        _databaseConnection.AccessLogs.Add(access);
                        _databaseConnection.SaveChanges();
                    }
                }

                HttpContext.Session.SetString("FrscQuestionLoggedInUser", JsonConvert.SerializeObject(userExist));
                if (userExist != null)
                    HttpContext.Session.SetString("FrscQuestionLoggedInUserId", userExist.AppUserId.ToString());
                if (userExist != null)
                {
                    var role = _databaseConnection.Roles.Find(userExist.RoleId);
                    if (role.AccessAdminConsole && role.ManageApplicationUser)
                    {
                        return RedirectToAction("Dashboard", "User");
                    }

                    return RedirectToAction("Index", "Home");
                }

                //display notification
                TempData["display"] = access.Message;
                TempData["notificationtype"] = NotificationType.Error.ToString();
                return View(model);
            }
            catch (Exception)
            {
                //display notification
                TempData["display"] = "Unable to Sign In. Try again!";
                TempData["notificationtype"] = NotificationType.Error.ToString();
                return View(model);
            }
        }

        /// <summary>
        ///     Log Out User from System
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult LogOut()
        {
            _databaseConnection.Dispose();
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}