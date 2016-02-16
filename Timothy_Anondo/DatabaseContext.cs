using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timothy_Anondo.Models;

namespace Timothy_Anondo
{
    public class DatabaseContext : DbContext
    {
        

        //static MySqlConnection con = new MySqlConnection(connectionString);


        static DatabaseContext()
        {
            //con.Open();
            

            Database.SetInitializer(new MySqlInitializer());
        }

        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Sales_Order> Sales_Orders { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Social_Traffic> Social_Traffics { get; set; }
        public virtual DbSet<Network_Details> Network_Details { get; set; }
        public virtual DbSet<User> Followers { get; set; }
        public virtual DbSet<Advertisement_Cost> Advertisement_Cost { get; set; }
        public virtual DbSet<Sales_Increment> Sales_Increments { get; set; }



        public DatabaseContext() : base("Timothy_Anondo") {}

        public DatabaseContext(DbConnection conn): base(conn, true){}

        //protected override void OnModelCreating(DbModelBuilder dbModelBuilder)
        //{
        //    dbModelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        //}
    }
}
