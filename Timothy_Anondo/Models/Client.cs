using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timothy_Anondo.Models
{
    [Table("Client")]
    public class Client
    {
        public int Id { get; set; }

        public string Company_Name { get; set; }

        private ICollection<Social_Traffic> Social_Traffics { get; set; }

        private ICollection<Sales_Order> Sales_Orders { get; set; }

        private ICollection<Customer> Customers { get; set; }
    }
}
