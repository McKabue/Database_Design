using System.ComponentModel.DataAnnotations.Schema;

namespace Timothy_Anondo.Models
{
    [Table("Advertisement_Cost")]
    public class Advertisement_Cost
    {
        public int Id { get; set; }

        public double Cost_In_Time { get; set; }

        public double Cost_In_Cash_Spent { get; set; }

        public double Cost_In_Other_Resources { get; set; }
    }
}