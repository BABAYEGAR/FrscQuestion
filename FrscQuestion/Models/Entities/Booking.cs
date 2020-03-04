using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrscQuestion.Models.Entities
{
    public class Booking : Transport
    {
        public string BookingNumber { get; set; }
        [Required]
        public string OffenderName { get; set; }
        [Required]
        public string OffenderPhoneNumber { get; set; }
        public string OffenseLocation { get; set; }
        public string OffenderEmail { get; set; }
        [Required]
        public string OfficerName { get; set; }
        [Required]
        public string OfficerServiceNumber { get; set; }
        public  string ItemSiezed { get; set; }
    }
}
