using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FrscQuestion.Models.Entities
{
    public class Role : Transport
    {
        public long RoleId { get; set; }

        [Required] public string Name { get; set; }

        [DisplayName("Manage Users")] public bool ManageApplicationUser { get; set; }

        [DisplayName("Manage Roles & Privileges")] public bool ManageRoles { get; set; }

        [DisplayName("AccessAdminConsole")] public bool AccessAdminConsole { get; set; }

        [DisplayName("Manage Access Logs")] public bool ManageAccessLogs { get; set; }

        [DisplayName("Add Offense")] public bool AddOffense { get; set; }
        [DisplayName("Edit Offense")] public bool EditOffense { get; set; }
        [DisplayName("Delete Offense")] public bool DeleteOffense { get; set; }
        [DisplayName("View Offense")] public bool ViewOffense { get; set; }
        [DisplayName("Add Question")] public bool AddQuestion { get; set; }
        [DisplayName("Edit Question")] public bool EditQuestion { get; set; }
        [DisplayName("Delete Question")] public bool DeleteQuestion { get; set; }
        [DisplayName("View Question")] public bool ViewQuestion { get; set; }
        [DisplayName("Add Answer")] public bool AddAnswer { get; set; }
        [DisplayName("Edit Answer")] public bool EditAnswer { get; set; }
        [DisplayName("Delete Answer")] public bool DeleteAnswer { get; set; }
        [DisplayName("View Answer")] public bool ViewAnswer { get; set; }
        [DisplayName("Add Booking")] public bool AddBooking { get; set; }
        [DisplayName("Edit Booking")] public bool EditBooking { get; set; }
        [DisplayName("Delete Booking")] public bool DeleteBooking { get; set; }
        [DisplayName("View Booking")] public bool ViewBooking { get; set; }       
        [DisplayName("Add Booking Offense")] public bool AddBookingOffense { get; set; }
        [DisplayName("Edit Booking Offense")] public bool EditBookingOffense { get; set; }
        [DisplayName("Delete Booking Offense")] public bool DeleteBookingOffense { get; set; }
        [DisplayName("View Booking Offense")] public bool ViewBookingOffense { get; set; }
        [DisplayName("Pay Booking")] public bool PayBooking { get; set; }
        [DisplayName("Manage Payments")] public bool ManagePayments { get; set; }

        [DisplayName("Manage FAQ")] public bool ManageFaq { get; set; }

        [DisplayName("Manage Terms & Conditions")]
        public bool ManageTermsandCondition { get; set; }

        [DisplayName("Manage Privacy Policy")] public bool ManagePrivacyPolicy { get; set; }

        [DisplayName("Manage System Settings")]
        public bool ManageSystemSetting { get; set; }
    }
}