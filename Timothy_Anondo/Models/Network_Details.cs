using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Timothy_Anondo.Models
{
    [Table("Network_Details")]
    public class Network_Details
    {
        public int Id { get; set; }

        private ICollection<User> Followers { get; set; }

        public int Advertisement_Cost_Id { get; set; }
        [ForeignKey("Advertisement_Cost_Id")]
        private Advertisement_Cost Advertisement_Cost { get; set; }

        public int Sales_Increment_Id { get; set; }
        [ForeignKey("Sales_Increment_Id")]
        private Sales_Increment Sales_Increment { get; set; }
    }
}