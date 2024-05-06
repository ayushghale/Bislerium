using Bislerium.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Bislerium.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Blogs")]
        public int? BlogId { get; set; }

        public Blogs? Blog { get; set; }

        [ForeignKey("BisleriumUser")]
        public string? UserId { get; set; }

        public BisleriumUser? User { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string NotificationText { get; set; }

        // bool to check if the notification is read or not
        // default value is false
        public bool IsRead { get; set; } = false;

        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public DateTime UpdatedDate { get; set; }
    }
}
