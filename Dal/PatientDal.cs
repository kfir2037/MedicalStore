using MedicalStore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MedicalStore.Dal
{
    public class PatientDal:DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Patient>().ToTable("Patients");
        }
        public DbSet<Patient> Patient { get; set; }

        public System.Data.Entity.DbSet<MedicalStore.Models.Request> Requests { get; set; }
    }
}