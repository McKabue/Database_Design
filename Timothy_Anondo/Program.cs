using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Timothy_Anondo
{
    class Program
    {
        private static bool paid = false;
        private static UnitOfWork _unitOfWork;
        private static DataEntryForm _DataEntryForm;
        private static string pass;

        public static UnitOfWork _UnitOfWork
        {
            get
            {
                _unitOfWork = new UnitOfWork();
                return _unitOfWork;
            }
            set
            {
              _unitOfWork = value;
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            Paid();
            
            //_UnitOfWork.Seed();
            Application.Run(_DataEntryForm);

            //Console.WriteLine("Press Enter to exit...");
            //Console.ReadLine();
        }

        private static void Paid()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!paid)
            {
                string value1 = Math.RandomNumber();
                string value2 = Math.RandomNumber();
                Console.WriteLine(string.Format("Pay {0} to remove these annoying lines...", "3,500/="));
                Console.WriteLine(string.Format("Becouse you have not paid fot this work, yet, you will be forced to do some mathematics \n Here it is: \n {0} + {1} =  \nWrite the answer below... \n", value1, value2));
                if (Console.ReadLine() == Math.DoMath(value1, value2))
                {
                    Console.WriteLine("GOOOOOOD....\n\n\n");
                    Console.WriteLine("Please wait while the database is loading.......");
                    _DataEntryForm = new DataEntryForm();
                    _DataEntryForm.Text = "You Have Not Paid For This.... [3,500/=]";
                }
                else
                {
                    Environment.Exit(0);
                }
            }
            else
            {
                Console.WriteLine("Please wait while the database is loading.......");
                _DataEntryForm = new DataEntryForm();
            }
        }

        public static DatabaseContext CreateConnection()
        {
            

            DbProviderFactory factory = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
            string connectionString = string.Format("Server=localhost; port=3306; Database=Timothy_Anondo; Uid=root; Pwd={0}", pass);
            DbConnection conn = factory.CreateConnection();
            conn.ConnectionString = connectionString;
            //conn.Open();

            DatabaseContext _context = null;
            try
            {
                _context = new DatabaseContext(conn);
                _context.Database.Connection.Open();
                return _context;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Enter MySQL Password...");
                pass = Console.ReadLine();
                try
                {
                    return CreateConnection();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            
        }
    }
}
