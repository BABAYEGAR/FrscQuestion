using System.ComponentModel.DataAnnotations;

namespace FrscQuestion.Models.Entities
{
    public class NewsLetter : Transport
    {
        public long NewsLetterId { get; set; }

        [Required] public string Subject { get; set; }

        [Required] public string Body { get; set; }
    }
}