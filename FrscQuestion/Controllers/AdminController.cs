using System;
using System.Linq;
using FrscQuestion.Models;
using FrscQuestion.Models.DataBaseConnections;
using FrscQuestion.Models.Encryption;
using FrscQuestion.Models.Entities;
using FrscQuestion.Models.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FrscQuestion.Controllers
{
    public class AdminController : Controller
    {
        private readonly FrscQuestionDataContext _databaseConnection;

        public AdminController(FrscQuestionDataContext databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        public ActionResult AccessLog()
        {
            var authorizedUser = new AppUser();
            if (HttpContext.Session.GetString("FrscQuestionLoggedInUser") != null)
            {
                var userString = HttpContext.Session.GetString("FrscQuestionLoggedInUser");
                authorizedUser = JsonConvert.DeserializeObject<AppUser>(userString);
            }

            if (!authorizedUser.Role.AccessAdminConsole ||
                !authorizedUser.Role.ManageAccessLogs)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            return View(_databaseConnection.AccessLogs.Include(n => n.AppUser).ToList());
        }

    }
}