using System.ComponentModel.DataAnnotations;

namespace FrscQuestion.Models.Entities
{
    public class Faq : Transport
    {
        public long FaqId { get; set; }

        [Required] public string Question { get; set; }

        [Required] public string Answer { get; set; }
    }
}