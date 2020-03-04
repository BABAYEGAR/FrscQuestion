using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FrscQuestion.Models;
using FrscQuestion.Models.DataBaseConnections;
using FrscQuestion.Models.Entities;
using FrscQuestion.Models.Enum;
using FrscQuestion.Models.Services;
using Microsoft.EntityFrameworkCore;

namespace FrscQuestion.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly FrscQuestionDataContext _databaseConnection;

        public HomeController(FrscQuestionDataContext databaseConnection, ILogger<HomeController> logger)
        {
            _logger = logger;
            _databaseConnection = databaseConnection;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult UnauthorizedAccess()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Faq()
        {
            return View(_databaseConnection.Faqs.ToList());
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(Contact contact)
        {
            try
            {
                new Mailer().SendContactEmail(new AppConfig().ContactHtml, contact);
                //display notification
                TempData["display"] = "You have successfully contacted Afriplugz. We will get back to you shortly!";
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
        public IActionResult PrivacyPolicy()
        {
            return View(_databaseConnection.PrivacyPolicies.FirstOrDefault());
        }

        public IActionResult Terms()
        {
            return View(_databaseConnection.TermsAndConditions.OrderByDescending(n => n.DateCreated).ToList()
                .FirstOrDefault());
        }

        /// <summary>
        ///     send contact email to support
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Subscription(string email)
        {
            try
            {
                if (_databaseConnection.Subscriptions.Where(n => n.Email == email).ToList().Count > 0)
                {
                    //display notification
                    TempData["display"] = "You already have an active subscription to Afriplugz's Newsletter!";
                    TempData["notificationtype"] = NotificationType.Error.ToString();
                    return RedirectToAction("Index");
                }

                var subscription = new Subscription
                {
                    DateCreated = DateTime.Now,
                    DateLastModified = DateTime.Now,
                    Status = "Active",
                    Name = email,
                    Email = email
                };
                _databaseConnection.Subscriptions.Add(subscription);
                _databaseConnection.SaveChanges();
                if (subscription.SubscriptionId > 0)
                {
                    //display notification
                    TempData["display"] = "You have successfully subscribed to Afriplugz's Newsletter!";
                    TempData["notificationtype"] = NotificationType.Success.ToString();
                    return RedirectToAction("Index");
                }

                //display notification
                TempData["display"] = "There was an issue subscribing to Afriplugz's  Newletter!";
                TempData["notificationtype"] = NotificationType.Error.ToString();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                //display notification
                TempData["display"] = "Request is unavailable, Try again Later!";
                TempData["notificationtype"] = NotificationType.Error.ToString();
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        ///     send contact email to support
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public IActionResult UnSubscription(string email)
        {
            try
            {
                if (_databaseConnection.Subscriptions.Where(n => n.Email == email).ToList().Count < 0)
                {
                    //display notification
                    TempData["display"] =
                        "You currently do not have an active subscription on Afriplugz to Opt out of!";
                    TempData["notificationtype"] = NotificationType.Error.ToString();
                    return RedirectToAction("Index");
                }

                var subscription = _databaseConnection.Subscriptions.SingleOrDefault(n => n.Email == email);
                if (subscription != null)
                {
                    subscription.Status = "InActive";
                    _databaseConnection.Entry(subscription).State = EntityState.Modified;
                    _databaseConnection.SaveChanges();
                }

                //display notification
                TempData["display"] = "You have successfully Unsubscribed from the Afriplugz Newsletter!";
                TempData["notificationtype"] = NotificationType.Success.ToString();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                //display notification
                TempData["display"] = "Request is unavailable, Try again Later!";
                TempData["notificationtype"] = NotificationType.Error.ToString();
                return RedirectToAction("Index");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
