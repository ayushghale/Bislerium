using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Bislerium.Areas.Identity.Data;


namespace Bislerium.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Blogs")]
        public int? BlogId { get; set; }

        public Blogs? Blog { get; set; }

        [ForeignKey("BisleriumUser")]
        public string? UserId { get; set; }

        public BisleriumUser? User { get; set; }

        public int? ParentCommentID { get; set; } // Nullable foreign key for parent comment

        [ForeignKey("ParentCommentID")]
        public Comment? ParentComment { get; set; } // Navigation property for parent comment

        [Required]
        public string CommentText { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
