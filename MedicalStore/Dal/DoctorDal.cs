using MedicalStore.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MedicalStore.Dal
{
    public class DoctorDal: DbContext
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DoctorDal"].ConnectionString);

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Doctor>().ToTable("Doctors");
        }
        public DbSet<Doctor> Doctor { get; set; }
    }
}