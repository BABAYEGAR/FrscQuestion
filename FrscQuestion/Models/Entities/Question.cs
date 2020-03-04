using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FrscQuestion.Models.Entities
{
    public class Question : Transport
    {
        public long QuestionId { get; set; }
        public string QuestionValue { get; set; }
        public long OffenseId { get; set; }
        [ForeignKey("OffenseId")]
        public Offense Offense { get; set; }
        public  List<Answer> Answers { get; set; }
        
    }
}
