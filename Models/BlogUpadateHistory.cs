using Bislerium.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bislerium.Models
{
    public class BlogUpadateHistory
    {
        public int Id { get; set; }

        [ForeignKey("Blogs")]
        public int? BlogId { get; set; }

        public Blogs? Blog { get; set; }

        [ForeignKey("BisleriumUser")]
        public string? UserId { get; set; }

        public BisleriumUser? User { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
