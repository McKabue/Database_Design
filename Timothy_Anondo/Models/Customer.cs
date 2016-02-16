using System.ComponentModel.DataAnnotations.Schema;

namespace Timothy_Anondo.Models
{
    [Table("Customer")]
    public class Customer
    {
        public int Id { get; set; }

        public int Age { get; set; }

        public string Gender { get; set; }

        public string Country { get; set; }

        public string Demography { get; set; }

        public bool Is_New_Customer { get; set; }

        public int Client_Id { get; set; }
        [ForeignKey("Client_Id")]
        private Client Client { get; set; }
    }
}