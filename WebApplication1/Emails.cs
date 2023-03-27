using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1
{
    public class Emails
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int email_id { get; set; }
        public string Address { get; set; }
        public DateTime date { get; set; }
        public string status { get; set; }
    }
}
