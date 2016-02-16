using System.ComponentModel.DataAnnotations.Schema;

namespace Timothy_Anondo.Models
{
    [Table("Sales_Increment")]
    public class Sales_Increment
    {
        public int Id { get; set; }

        public string Increment_By_New_Orders { get; set; }
    }
}