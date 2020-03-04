using FrscQuestion.Models.Entities;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace FrscQuestion.Models.Encryption
{
    public class HangFireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var isConnected = false;
            var httpContext = context.GetHttpContext();

            // Allow all authenticated users to see the Dashboard (potentially dangerous).
            if (httpContext.Session.GetString("FrscQuestionLoggedInUser") != null)
            {
                var userString = httpContext.Session.GetString("FrscQuestionLoggedInUser");
                var appUser = JsonConvert.DeserializeObject<AppUser>(userString);
                if (appUser != null && appUser.Role.AccessAdminConsole && appUser.Role.ManageApplicationUser) isConnected = true;
            }

            return isConnected;
        }
    }
}