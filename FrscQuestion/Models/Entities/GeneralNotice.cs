using System.ComponentModel.DataAnnotations;

namespace FrscQuestion.Models.Entities
{
    public class GeneralNotice : Transport
    {
        public long GeneralNoticeId { get; set; }

        [Required] public string Subject { get; set; }

        [Required] public string Body { get; set; }
    }
}