﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MedicalStore.Models
{
    public class Patient
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [Key]
        [StringLength(9, ErrorMessage = "Id length have to be 9 numbers")]
        [RegularExpression("([0-9]{9})", ErrorMessage = "ID must contain 9 numbers")]
        public string Id { get; set; }
        [Required]
        public string Age { get; set; }
        [Required]
        public string City { get; set; }
        public string UserCode { get; set; }
        public string Prescription { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z0-9]{8,16}$", ErrorMessage = "Password must contain between 8 to 16 chars or numbers")]
        public string Password { get; set; }
        [Required]
        public string UserName { get; set; }
        public bool Blocked { get; set; }
        public int LoginAttempts { get; set; }

    }
}