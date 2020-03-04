using System.Collections.Generic;

namespace FrscQuestion.Models.Entities
{
    public class AppTransport
    {
        public string Page { get; set; }
        public List<Payment> Payments { get; set; }
        public List<Booking> Bookings { get; set; }
        public List<AppUser> AppUsers { get; set; }
        public AppUser AppUser { get; set; }
        public string PaymentReference { get; set; }
        public string Order { get; set; }
        public decimal Amount { get; set; }
        public List<TermAndCondition> TermAndConditions { get; set; }
        public PrivacyPolicy PrivacyPolicy { get; set; }
        public AppCredential AppCredential { get; set; }
    }
}