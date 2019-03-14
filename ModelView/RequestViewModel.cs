using MedicalStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedicalStore.ModelView
{
    public class RequestViewModel
    {
        public Request request { get; set; }
        public List<Request> requests { get; set; }
    }
}