using MedicalStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedicalStore.ModelView
{
    public class DoctorViewModel
    {
        public Doctor doctor { get; set; }
        public List<Doctor> doctors { get; set; }
    }
}