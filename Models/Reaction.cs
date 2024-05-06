using Bislerium.Areas.Identity.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bislerium.Models
{
    public class Reaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [ForeignKey("Blog")]
        public int BlogId { get; set; }

        public Blogs Blog { get; set; }

        [ForeignKey("Comment")]
        public int? commentId { get; set; }

        public Comment Comment { get; set; }


        [ForeignKey("BisleriumUser")]
        public string UserID { get; set; }


        public BisleriumUser User { get; set; }

        public int? Upvote { get; set; }

        public int? Downvote { get; set; }


        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
