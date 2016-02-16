using System.ComponentModel.DataAnnotations.Schema;

namespace Timothy_Anondo.Models
{
    [Table("Sales_Order")]
    public class Sales_Order
    {
        public int Id { get; set; }

        public string Type_Of_Order { get; set; }

        public bool Is_New_Order { get; set; }

        public int Client_Id { get; set; }
        [ForeignKey("Client_Id")]
        private Client Client { get; set; }
    }
}