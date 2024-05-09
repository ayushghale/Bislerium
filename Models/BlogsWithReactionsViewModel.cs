namespace Bislerium.Models
{
    public class BlogsWithReactionsViewModel
    {
        public Blogs Blog { get; set; } = default!;

        public int UpvoteCount { get; set; }

        public int DownvoteCount { get; set; }

        public bool IsUpvoted { get; set; }

        public bool IsDownvoted { get; set; }

        public int CommentCount { get; set; }
    }
}
