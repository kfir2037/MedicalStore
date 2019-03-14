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
    public class RequestDal:DbContext
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["RequestDal"].ConnectionString);
        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //    modelBuilder.Entity<Request>().ToTable("Order");
        //}
        public DbSet<Request> Requests { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<RequestDal>(null);
            base.OnModelCreating(modelBuilder);
        }
    }

}