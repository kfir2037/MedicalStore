using MedicalStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedicalStore.ModelView
{
    public class PatientViewModel
    {
        public Patient patient { get; set; }
        public List<Patient> patients { get; set; }
    }
}