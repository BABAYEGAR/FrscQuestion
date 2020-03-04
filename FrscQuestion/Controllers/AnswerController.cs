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
    public class AnswerController : Controller
    {
        private readonly FrscQuestionDataContext _databaseConnection;

        public AnswerController(FrscQuestionDataContext databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        // GET: ItemCategory
        [SessionExpireFilter]
        public ActionResult Index(long id)
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

            ViewBag.QuestionId = id;
            return View(_databaseConnection.Answers.Where(n=>n.QuestionId == id).ToList());
        }

        // GET: ItemCategory/Create
        [SessionExpireFilter]
        public ActionResult Create(long id)
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
            var answer = new Answer();
            answer.QuestionId = id;
            return View(answer);
        }

        // POST: ImageCategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpireFilter]
        public ActionResult Create(Answer answer)
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
                answer.DateCreated = DateTime.Now;
                answer.DateLastModified = DateTime.Now;
                answer.CreatedBy = signedInUserId;
                answer.LastModifiedBy = signedInUserId;
                if (_databaseConnection.Answers.Where(n => n.AnswerValue == answer.AnswerValue).ToList().Count > 0)
                {
                    //display notification
                    TempData["display"] = "Unable to perform the action because this record already exist!";
                    TempData["notificationtype"] = NotificationType.Error.ToString();
                    return View(answer);
                }

                _databaseConnection.Answers.Add(answer);
                _databaseConnection.SaveChanges();

                //display notification
                TempData["display"] = "You have successfully added a new Answer!";
                TempData["notificationtype"] = NotificationType.Success.ToString();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //display notification
                TempData["display"] = ex.Message;
                TempData["notificationtype"] = NotificationType.Error.ToString();
                return View(answer);
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
                !authorizedUser.Role.EditAnswer)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            return View(_databaseConnection.Answers.Find(id));
        }

        // POST: ImageCategory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpireFilter]
        public ActionResult Edit(Answer answer)
        {
            var authorizedUser = new AppUser();
            if (HttpContext.Session.GetString("FrscQuestionLoggedInUser") != null)
            {
                var userString = HttpContext.Session.GetString("FrscQuestionLoggedInUser");
                authorizedUser = JsonConvert.DeserializeObject<AppUser>(userString);
            }

            if (!authorizedUser.Role.AccessAdminConsole ||
                !authorizedUser.Role.EditAnswer)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            try
            {
                // TODO: Add update logic here
                var signedInUserId = Convert.ToInt64(HttpContext.Session.GetString("FrscQuestionLoggedInUserId"));
                answer.DateLastModified = DateTime.Now;
                answer.LastModifiedBy = signedInUserId;
                if (_databaseConnection.Answers.Where(n => n.AnswerValue == answer.AnswerValue && n.AnswerId != answer.AnswerId).ToList()
                        .Count > 0)
                {
                    //display notification
                    TempData["display"] = "Unable to perform the action because this record already exist!";
                    TempData["notificationtype"] = NotificationType.Error.ToString();
                    return View(answer);
                }

                _databaseConnection.Entry(answer).State = EntityState.Modified;
                _databaseConnection.SaveChanges();

                //display notification
                TempData["display"] = "You have successfully modified the Answer!";
                TempData["notificationtype"] = NotificationType.Success.ToString();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //display notification
                TempData["display"] = ex.Message;
                TempData["notificationtype"] = NotificationType.Error.ToString();
                return View(answer);
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
                !authorizedUser.Role.DeleteAnswer)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            var id = Convert.ToInt64(collection["Id"]);
            var answer = _databaseConnection.Answers.Find(id);

            _databaseConnection.Answers.Remove(answer);
            _databaseConnection.SaveChanges();

            //display notification
            TempData["display"] = "You have successfully deleted the Answer!";
            TempData["notificationtype"] = NotificationType.Success.ToString();
            return RedirectToAction("Index");
        }
    }
}