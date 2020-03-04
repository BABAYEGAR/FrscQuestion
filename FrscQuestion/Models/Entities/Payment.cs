using System.ComponentModel.DataAnnotations.Schema;

namespace FrscQuestion.Models.Entities
{
    public class Payment : Transport
    {
        public long PaymentId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public long? AppUserId { get; set; }

        [ForeignKey("AppUserId")] 
        public AppUser AppUser { get; set; }

        public string Reference { get; set; }
        public string Status { get; set; }
        public string PaymentGateway { get; set; }
    }
}