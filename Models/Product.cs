using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MedicalStore.Models
{
    public class Product
    {
        [Key]
        [MaxLength(10)]
        [Required]
        public string MedId { get; set; }
        [Required]
        public string MedName { get; set; }
        [Required]
        public string Price { get; set; }
        [MaxLength(50)]
        public string Description { get; set; }
    }
}