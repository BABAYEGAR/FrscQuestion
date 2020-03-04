using System.ComponentModel.DataAnnotations;

namespace FrscQuestion.Models.Entities
{
    public class TermAndCondition : Transport
    {
        public long TermAndConditionId { get; set; }

        [Required] public string Text { get; set; }
    }
}