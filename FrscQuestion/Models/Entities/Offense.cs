using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrscQuestion.Models.Entities
{
    public class Offense : Transport
    {
        public long OffenseId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public decimal Fine { get; set; }
    }
}
