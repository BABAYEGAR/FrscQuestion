using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace FrscQuestion.Models.Entities
{
    public class AppUserAccessKey : Transport
    {
        public long AppUserAccessKeyId { get; set; }
        public long? AppUserId { get; set; }

        [ForeignKey("AppUserId")] [JsonIgnore] public AppUser AppUser { get; set; }

        public string PasswordAccessCode { get; set; }
        public string AccountActivationAccessCode { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}