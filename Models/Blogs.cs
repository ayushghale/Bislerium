using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Bislerium.Areas.Identity.Data;

namespace Bislerium.Models
{
    public class Blogs
    {
        [Key] // Define primary key
        public int Id { get; set; }

        [ForeignKey("user")]
        public string? userId { get; set; }

        public BisleriumUser? User { get; set; }

        public string? Image { get; set; }

        [Required]
        public string Heading { get; set; }

        [Required]
        public string Description { get; set; }


        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
