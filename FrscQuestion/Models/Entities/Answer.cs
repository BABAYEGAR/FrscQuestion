using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FrscQuestion.Models.Entities
{
    public class Answer : Transport
    {
        public long AnswerId { get; set; }
        public string AnswerValue { get; set; }
        public long QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public Question Question { get; set; }
        public bool CorrectAnswer { get; set; }
    }
}
