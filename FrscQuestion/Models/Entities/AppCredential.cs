using System.ComponentModel.DataAnnotations;

namespace FrscQuestion.Models.Entities
{
    public class AppCredential : Transport
    {
        public long AppCredentialId { get; set; }

        [Display(Name = "Event Planner ID")]
        [Required]
        public long EventPlannerId { get; set; }

        [Display(Name = "Customer ID")]
        [Required]
        public long CustomerId { get; set; }

        [Display(Name = "Paystack Secret Key")]
        [Required]
        public string PaystackSecretKey { get; set; }

        [Display(Name = "Paystack Public key")]
        [Required]
        public string PaystackPublicKey { get; set; }

        [Display(Name = "Default Vendor Payment Percentage (%)")]
        [Required]
        public decimal DefaultVendorPaymentPercentage { get; set; }
    }
}