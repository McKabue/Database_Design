using System.ComponentModel.DataAnnotations.Schema;

namespace Timothy_Anondo.Models
{
    [Table("Social_Traffic")]
    public class Social_Traffic
    {
        public int Id { get; set; }

        public string Network_Name { get; set; }

        public int Network_Details_Id { get; set; }
        [ForeignKey("Network_Details_Id")]
        private Network_Details Network_Details { get; set; }

        public int Client_Id { get; set; }
        [ForeignKey("Client_Id")]
        private Client Client { get; set; }
    }
}