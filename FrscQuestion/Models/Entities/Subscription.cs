using System.ComponentModel.DataAnnotations;

namespace FrscQuestion.Models.Entities
{
    public class Subscription : Transport
    {
        public long SubscriptionId { get; set; }

        [Required] public string Name { get; set; }

        [EmailAddress] [Required] public string Email { get; set; }

        public string Status { get; set; }
    }
}