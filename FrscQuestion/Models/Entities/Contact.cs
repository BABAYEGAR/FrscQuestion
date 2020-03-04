using System.ComponentModel.DataAnnotations;

namespace FrscQuestion.Models.Entities
{
    public class Contact
    {
        [Required] public string Name { get; set; }

        [Required] [EmailAddress] public string Email { get; set; }

        [Required] public string Mobile { get; set; }

        [Required] public string Subject { get; set; }

        [Required] public string Message { get; set; }
    }
}