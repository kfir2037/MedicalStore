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
    public class ProductDal: DbContext
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ProductDal"].ConnectionString);


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().ToTable("Products");
        }
        public DbSet<Product> Product { get; set; }
    }
}