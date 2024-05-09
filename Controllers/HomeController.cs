using Bislerium.Data;
using Bislerium.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bislerium.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BisleriumContext _context;

        public HomeController(ILogger<HomeController> logger, BisleriumContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get recent 5 blogs
            var blogs = _context.Blogs
                .Include(b => b.User)
                .OrderByDescending(b => b.CreatedDate)
                .Take(4)
                .ToList();

            var blogsWithCounts = blogs.Select(blog =>
            {
                var reactions = _context.Reaction.Where(r => r.BlogId == blog.Id);

                return new BlogsWithReactionsViewModel
                {
                    Blog = blog,
                    UpvoteCount = reactions.Count(r => r.Upvote != null && r.Upvote > 0),
                    DownvoteCount = reactions.Count(r => r.Downvote != null && r.Downvote > 0),
                    IsUpvoted = reactions.Any(r => r.UserID == userId && r.Upvote != null && r.Upvote > 0),
                    IsDownvoted = reactions.Any(r => r.UserID == userId && r.Downvote != null && r.Downvote > 0),
                };
            }).ToList();

            // Get top 5 blogs
            var topFiveBlogs = await _context.Blogs
                .Include(b => b.User)
                .GroupJoin(
                    _context.Reaction,
                    blog => blog.Id,
                    reaction => reaction.BlogId,
                    (blog, reactions) => new
                    {
                        Blog = blog,
                        Reactions = reactions,
                    }
                )
                .Select(b => new BlogsWithReactionsViewModel
                {
                    Blog = b.Blog,
                    UpvoteCount = b.Reactions.Where(r => r.Upvote != null && r.Upvote > 0).Count(), // Count upvotes
                    DownvoteCount = b.Reactions.Where(r => r.Downvote != null && r.Downvote > 0).Count(), // Count downvotes
                    IsUpvoted = b.Reactions.Any(r => r.UserID == userId && r.Upvote != null && r.Upvote > 0), // Check if the user has upvoted
                    IsDownvoted = b.Reactions.Any(r => r.UserID == userId && r.Downvote != null && r.Downvote > 0), // Check if the user has downvoted
                    CommentCount = _context.Comment.Count(comment => comment.BlogId == b.Blog.Id) // Count comments
                })
                .OrderByDescending(b => b.UpvoteCount) // Order by upvote count in descending order
                .Take(5) // Take the top five blogs
                .ToListAsync();

            TempData["RecentBlogs"] = blogsWithCounts;
            TempData["TopFiveBlogs"] = topFiveBlogs;

            return View();
        }


        public async Task<IActionResult> Blogs()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var blogsWithCounts = await _context.Blogs
                .Include(b => b.User)
                .GroupJoin(
                    _context.Reaction,
                    blog => blog.Id,
                    reaction => reaction.BlogId,
                    (blog, reactions) => new
                    {
                        Blog = blog,
                        Reactions = reactions,
                    }
                )
                .Select(b => new BlogsWithReactionsViewModel
                {
                    Blog = b.Blog,
                    UpvoteCount = b.Reactions.Where(r => r.Upvote != null && r.Upvote > 0).Count(), // Count upvotes
                    DownvoteCount = b.Reactions.Where(r => r.Downvote != null && r.Downvote > 0).Count(), // Count downvotes
                    IsUpvoted = b.Reactions.Any(r => r.UserID == userId && r.Upvote != null && r.Upvote > 0), // Check if the user has upvoted
                    IsDownvoted = b.Reactions.Any(r => r.UserID == userId && r.Downvote != null && r.Downvote > 0), // Check if the user has downvoted
                    CommentCount = _context.Comment.Count(comment => comment.BlogId == b.Blog.Id) // Count comments
                })
                .ToListAsync();

            return View(blogsWithCounts);
        }


        public async Task<IActionResult> BlogDescription(int id)
        {
            // Retrieve the blog from the database using its ID and include the User navigation property
            var blog = await _context.Blogs
                .Include(b => b.User) // Include the User navigation property
                .FirstOrDefaultAsync(b => b.Id == id);



            if (blog == null)
            {
                // If the blog with the provided ID is not found, return a not found error
                return NotFound();
            }

            // Pass the retrieved blog to the view
            return View(blog);
        }





        public IActionResult Description()
        {
            return View();
        }

        public async Task<IActionResult> FetchComment(int blogId)
        {
            var comments = await _context.Comment
                .Include(c => c.User)
                .Where(c => c.BlogId == blogId)
                .ToListAsync();


            return Ok(comments);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
