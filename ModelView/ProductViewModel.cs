using MedicalStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedicalStore.ModelView
{
    public class ProductViewModel
    {
        public Product product { get; set; }
        public List<Product> products { get; set; }
        public Request request { get; set; }
        public Patient patient { get; set; }
        public Doctor doctor { get; set; }
        public List<Doctor> doctors { get; set; }
        public List<String> doctorsNames { get; set; }
        public List<String> productsNames { get; set; }

    }
}