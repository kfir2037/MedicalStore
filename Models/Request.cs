using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MedicalStore.Models
{
    public class Request
    {
        [Key]
        public int RequestNumber { get; set; }
        [Required]
        public string PatientId { get; set; }
        [Required]
        public string MedId { get; set; }
        public bool Approve { get; set; }
        [Required]
        public string DocId { get; set; }

    }
}