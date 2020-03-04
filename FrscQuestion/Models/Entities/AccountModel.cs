using System.ComponentModel.DataAnnotations;

namespace FrscQuestion.Models.Entities
{
    public class AccountModel
    {
        [Required]
        [Display(Name = "Personal/Business Name")]
        public string LoginName { get; set; }

        [Required] public string UserName { get; set; }
        [Display(Name = "Email Address")]
        [Required] [EmailAddress] public string Email { get; set; }
        [Display(Name = "Phone Number")]
        [Phone]
        [Required] public string Mobile { get; set; }

        [Required] public string Password { get; set; }

        [Required]
        [Compare("Password")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        public long? AppUserId { get; set; }
        public long? RoleId { get; set; }
        public string LoginType { get; set; }
        public string ProfilePicture { get; set; }
        public string Address { get; set; }
    }
}