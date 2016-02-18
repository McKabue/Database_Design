using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timothy_Anondo.Models;

namespace Timothy_Anondo
{
    public class UnitOfWork : IDisposable
    {
        //public object this[string propertyName]
        //{
        //    get { return this.GetType().GetProperty(propertyName).GetValue(this, null); }
        //    set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
        //}


        private DatabaseContext context;
        
        public UnitOfWork()
        {
            context = Program.CreateConnection();
            //context = new DatabaseContext();
        }
        




        public IEnumerable<string> GetTables()
        {
            var metadata = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;

            //http://romiller.com/2012/04/20/what-tables-are-in-my-ef-model-and-my-database/
            var tables = metadata.GetItemCollection(DataSpace.SSpace)
              .GetItems<EntityContainer>()
              .Single()
              .BaseEntitySets
              .OfType<EntitySet>()
              .Where(s => !s.MetadataProperties.Contains("Type")
                || s.MetadataProperties["Type"].ToString() == "Tables");

            foreach (var table in tables)
            {
                string tableName = table.MetadataProperties.Contains("Table")
                    && table.MetadataProperties["Table"].Value != null
                  ? table.MetadataProperties["Table"].Value.ToString()
                  : table.Name;
                yield return tableName;
                //var tableSchema = table.MetadataProperties["Schema"].Value.ToString();

               // Console.WriteLine(tableSchema + "." + tableName);
            }

            //return context.Database.SqlQuery<string>("SELECT table_name from information_schema.tables WHERE table_schema='Timothy_Anondo' and not (table_name = '__migrationhistory')").ToList();
           
        }

        public void Seed()
        {
            //context.Users.AddRange(new User[] {
            //  new User { FirstName = "Andrew Peters" },
            //  new User { FirstName = "Brice Lambson" },
            //  new User { FirstName = "Rowan Miller" }
            //});
            //context.SaveChanges();
        }

        public void Save()
        {
            context.SaveChanges();
            context.RefreshAllEntites(RefreshMode.StoreWins);
        }
        
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public object RawSql(string table)
        {
            Type u = Type.GetType(string.Format("Timothy_Anondo.Models.{0}", table), true, true);
            MethodInfo method = typeof(System.Data.Entity.Database).GetMethod("SqlQuery", new[] { typeof(string), typeof(object[]) });
            MethodInfo generic = method.MakeGenericMethod(u);
            try
            {
                return generic.Invoke(context.Database, new object[] { string.Format("SELECT * FROM {0}.{1}", Program.dbname, table), new object[] { } });

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw e;
            }
            //string query = string.Format("SELECT * FROM {0} AS item", table);
            //ObjectQuery<DbDataRecord> customersQuery = new ObjectQuery<DbDataRecord>(query, ((IObjectContextAdapter)context).ObjectContext);

            ////lblQueryString.Text = query;

            //var tinyCustomers = customersQuery.ToList();//.ConvertTo<DemoTypes.TinyCustomer>();

            //return context.Database.SqlQuery<DbDataRecord>(string.Format("SELECT * FROM {0}.{1}", "timothy_anondo", table));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void ExecuteSqlCommand(string name, List<KeyValuePair<string, string>> data)
        {
            StringBuilder strkeys = new StringBuilder();
            StringBuilder strvalues = new StringBuilder();
            strkeys.Append("(");
            strvalues.Append("(");
            for (int i=0; i<data.Count; i++)
            {
                KeyValuePair<string, string> keyvalue = data[i];
                strkeys.Append(string.Format("`{0}`", keyvalue.Key));
                strvalues.Append(string.Format("'{0}'", keyvalue.Value));
                if (i<(data.Count -1))
                {
                    strkeys.Append(",");
                    strvalues.Append(","); 
                }
            }
            strkeys.Append(")");
            strvalues.Append(")");
            string k = strkeys.ToString();
            string v = strvalues.ToString();







            string query = string.Format("INSERT INTO `{0}`.`{1}` {2} VALUES {3}", Program.dbname, name, k, v);
            try
            {
                context.Database.ExecuteSqlCommand(query);
                MessageBox.Show("Data Saved...");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                context.Dispose();
                context = Program.CreateConnection();
            }

        }
    }
}
