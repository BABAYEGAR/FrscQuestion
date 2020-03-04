using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FrscQuestion.Models.Entities
{
    public class Transport
    {
        [DisplayName("Created By")] public long? CreatedBy { get; set; }

        [DisplayName("Date Created")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateCreated { get; set; }

        [DisplayName("Date Last Modified")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateLastModified { get; set; }

        [DisplayName("Last Modified By")] public long? LastModifiedBy { get; set; }
    }
}