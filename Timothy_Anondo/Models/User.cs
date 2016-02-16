using System.ComponentModel.DataAnnotations.Schema;

namespace Timothy_Anondo.Models
{
    [Table("User")]
    public class User
    {
        public int Id { get; set; }
        
        public int Age { get; set; }

        public string Gender { get; set; }

        public string Country { get; set; }

        public string Demography { get; set; }

        public int Network_Details_Id { get; set; }
        [ForeignKey("Network_Details_Id")]
        private Network_Details Network_Details { get; set; }
    }
}