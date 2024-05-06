using Bislerium.Data;
using Bislerium.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Execution;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System;
using System.Linq;
using System.Security.Claims;

namespace Bislerium.Controllers
{
    public class UserController : Controller
    {
        private readonly BisleriumContext _context;

        public UserController(BisleriumContext context)
        {
            _context = context;

        }

        public async Task<IActionResult> Dashboard()
        {
            // Retrieve blogs associated with the current user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var blogs = _context.Blogs.Where(b => b.userId == userId).ToList();

            // Retrieve the user from the database
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            var UserVlogData = await _context.Blogs
                .Where(b => b.userId == userId)
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

            // Add other properties as needed

            // Store the user object in TempData
            TempData["user"] = user;
            TempData["UserVlogData"] = UserVlogData;

            // Return the blogs and user object to the view
            return View(UserVlogData);
        }


        // user blogs crud ================================================

        // create blog
        // POST: Blogs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("User/CreateBlog")]
        public async Task<IActionResult> CreateBlog([Bind("Id,Heading,Description")] Blogs blogs, IFormFile Image)
        {
            Console.WriteLine("Create Blog");

            if (ModelState.IsValid)
            {

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Assign the current user's ID to the UserId property of the post
                blogs.userId = userId;

                if (Image != null && Image.Length > 0)
                {
                    // Check if the file size exceeds the limit (3 MB)
                    if (Image.Length > 3 * 1024 * 1024) // 3 MB in bytes
                    {
                        ModelState.AddModelError("Image", "The image must be less than 3 Megabytes (MB).");
                        return RedirectToAction(nameof(Dashboard));
                    }

                    // Process the uploaded image, such as saving it to the file system or database
                    // For simplicity, let's assume you have a method to handle saving the image
                    blogs.Image = await SaveImage(Image);
                }
                else
                {

                    blogs.Image = "drxgrddrcx";
                }

                blogs.UpdatedDate = DateTime.Now;
                blogs.CreatedDate = DateTime.Now;
                _context.Add(blogs);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Dashboard));
            }
            ViewData["userId"] = new SelectList(_context.Users, "Id", "Id", blogs.userId);
            return RedirectToAction(nameof(Dashboard));
        }

        // save image
        private async Task<string> SaveImage(IFormFile Image)
        {

            Console.WriteLine("Save Image");
            var fileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(Image.FileName)}";
            Console.WriteLine(fileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await Image.CopyToAsync(stream);
            }

            // Return the URL or path of the saved Image
            return $"/img/{fileName}";
        }

        // edit blog
        public async Task<IActionResult> EditBlog(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // to check if the user is the owner of the blog
            var blogExist = await _context.Blogs
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id && m.userId == userId);
            if (blogExist == null)
            {
                return NotFound();
            }

            return View(blogExist);
        }

        // POST: Blogs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBlog(int id, [Bind("Id,Heading,Description")] Blogs blogs, IFormFile Image)
        {
            if (id != blogs.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    // to check if the user is the owner of the blog
                    var blogExist = await _context.Blogs
                        .Include(b => b.User)
                        .FirstOrDefaultAsync(m => m.Id == id && m.userId == userId);
                    if (blogExist == null)
                    {
                        return NotFound();
                    }

                    // Update the properties of the existing blog
                    blogExist.Heading = blogs.Heading;
                    blogExist.Description = blogs.Description;

                    if (Image != null && Image.Length > 0)
                    {
                        // Process the uploaded image, such as saving it to the file system or database
                        blogExist.Image = await SaveImage(Image);
                    }

                    blogExist.UpdatedDate = DateTime.Now;
                    _context.Update(blogExist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogsExists(blogs.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Dashboard));
            }
            ViewData["userId"] = new SelectList(_context.Users, "Id", "Id", blogs.userId);
            return View(blogs);
        }

        private bool BlogsExists(int id)
        {
            throw new NotImplementedException();
        }

        // delete blog
        public async Task<IActionResult> DeleteBlog(int? id)
        {
            Console.WriteLine("Delete");
            if (id == null)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // to check if the user is the owner of the blog
            var blogExist = await _context.Blogs
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id && m.userId == userId);
            if (blogExist == null)
            {
                return NotFound();
            }

            if (blogExist != null)
            {
                _context.Blogs.Remove(blogExist);
                await _context.SaveChangesAsync();
            }
            var blogs = _context.Blogs.Where(b => b.userId == userId).ToList();

            return RedirectToAction(nameof(Dashboard));
        }


        // user reactions ================================================
        // POST: User/AddBlogUpVote/5

        [HttpPost]
        [Route("User/AddBlogUpVote/{blogId}")]
        public async Task<IActionResult> AddBlogUpVote([Bind("ID,BlogId")] Reaction reaction, int BlogId)
        {
            reaction.UserID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            reaction.UpdatedDate = DateTime.Now;



            // Check if the user has already upvoted or downvoted the post
            var existingReaction = await _context.Reaction.FirstOrDefaultAsync(r => r.BlogId == reaction.BlogId && r.UserID == reaction.UserID);

            if (existingReaction != null)
            {
                // If the user has already upvoted, return a bad request
                if (existingReaction.Upvote.HasValue)
                {
                    return BadRequest("You have already upvoted this blog.");
                }
                else
                {
                    // If the user has downvoted, remove the downvote and update with upvote
                    existingReaction.Downvote = null;
                    existingReaction.Upvote = 1;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                // User hasn't reacted to this post yet, add new reaction
                reaction.Downvote = null;
                reaction.Upvote = 1;
                reaction.CreatedDate = DateTime.Now;
                _context.Add(reaction);
                await _context.SaveChangesAsync();

            }
            // add notification for upvote
            var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == reaction.BlogId);
            if (blog != null)
            {
                var notificationText = "New upvote on your blog: " + blog.Heading;
                await AddNotification(notificationText, reaction.UserID, reaction.BlogId, "upvote");
            }
            return Ok(new { message = "Upvote added successfully." });

        }

        // POST: User/AddBlogDownVote/5
        [HttpPost]
        [Route("User/AddBlogDownVote/{blogId}")]
        public async Task<IActionResult> AddBlogDownVote([Bind("ID,BlogId")] Reaction reaction, int BlogId)
        {
            reaction.UserID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            reaction.UpdatedDate = DateTime.Now;

            // Check if the user has already upvoted or downvoted the post
            var existingReaction = await _context.Reaction.FirstOrDefaultAsync(r => r.BlogId == reaction.BlogId && r.UserID == reaction.UserID);
            if (existingReaction != null)
            {
                // If the user has already upvoted, return a bad request
                if (existingReaction.Downvote.HasValue)
                {
                    return BadRequest("You have already downvoted this blog.");
                }
                else
                {
                    // If the user has downvoted, remove the downvote and update with upvote
                    existingReaction.Upvote = null;
                    existingReaction.Downvote = 1;
                    await _context.SaveChangesAsync();

                }
            }
            else
            {
                // User hasn't reacted to this post yet, add new reaction
                reaction.Upvote = null;
                reaction.Downvote = 1;
                reaction.CreatedDate = DateTime.Now;
                _context.Add(reaction);
                await _context.SaveChangesAsync();
            }
            // add notification for downvote
            var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == reaction.BlogId);
            if (blog != null)

            {
                var notificationText = "New downvote on your blog: " + blog.Heading;
                await AddNotification(notificationText, reaction.UserID, reaction.BlogId, "downvote");
            }
            return Ok(new { message = "DownVote added successfully." });
        }


        // add comment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment([Bind("ID,BlogId,Comment")] Comment comment, int BlogId)
        {
            comment.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            comment.CreatedDate = DateTime.Now;
            _context.Add(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // delete comment
        public async Task<IActionResult> DeleteComment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // to check if the user is the owner of the comment
            var commentExist = await _context.Comment
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (commentExist == null)
            {
                return NotFound();
            }

            if (commentExist != null)
            {
                _context.Comment.Remove(commentExist);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        // notifcations
        public async Task<IActionResult> Notifications()
        {
            Console.WriteLine("Notifications=====================================");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // get all notifications
            var notifications = _context.Notification.Include(n => n.Blog).Include(n => n.User);

            // git notifications wher blogs user id
            var ReciverNotifications = _context.Notification.Include(n => n.Blog).Include(n => n.User).Where(n => n.Blog.User.Id == userId).ToList();


            foreach (var notification in ReciverNotifications)
            {
                Console.WriteLine("Notifications=====================================");
                Console.WriteLine(notification.NotificationText);
                Console.WriteLine("Notifications=====================================");
            }


            return Ok(ReciverNotifications);
        }

        // Mark the notification as read
        // post: User/MarkAsRead/5
        [HttpPost]
        [Route("User/MarkAsRead/{notificationId}")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Console.WriteLine("MarkAsRead=====================================");
            Console.WriteLine(notificationId);
            Console.WriteLine("MarkAsRead=====================================");

            // Get the notification
            var notification = await _context.Notification.FirstOrDefaultAsync(n => n.Id == notificationId);

            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Notification marked as read." });
            }

            return Ok(new { message = "Notification not found." });
        }

        // count unread notifications
        public Task<IActionResult> UnreadNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // get all notifications
            var notifications = _context.Notification.Include(n => n.Blog).Include(n => n.User);

            // git notifications wher blogs user id
            var ReciverNotifications = _context.Notification.Include(n => n.Blog).Include(n => n.User).Where(n => n.Blog.User.Id == userId).ToList();

            // Count the unread notifications
            var unreadCount = ReciverNotifications.Count(n => !n.IsRead);

            return Task.FromResult<IActionResult>(Ok(new { count = unreadCount }));
        }


        // comment ================================================
        // add comment
        [HttpPost]
        [Route("User/AddComment/{blogId}")]
        public async Task<IActionResult> Create([Bind("Id,BlogId,CommentText")] Comment comment)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            comment.UserId = userId;
            comment.CreatedDate = DateTime.Now;
            comment.UpdatedDate = DateTime.Now;

            if (ModelState.IsValid)
            {

                _context.Add(comment);
                await _context.SaveChangesAsync();

                // call notification
                var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == comment.BlogId);

                var notificationText = "New comment on your blog: " + blog.Heading;

                var BlogId = comment.BlogId;

                await AddNotification(notificationText, userId, BlogId, "comment");




                return Ok(new { message = "Comment added successfully." });
            }
            return Ok(new { message = "Comment not added." });
        }

        // add sub comment
        [HttpPost]
        [Route("User/AddSubComment/{ParentCommentID}")]
        public async Task<IActionResult> AddSubComment([Bind("Id,BlogId,ParentCommentID,CommentText")] Comment subComment)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            subComment.UserId = userId;
            subComment.CreatedDate = DateTime.Now;
            subComment.UpdatedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Add(subComment);
                await _context.SaveChangesAsync();
                return Ok(new { message = "SubComment added successfully." });
            }
            return Ok(new { message = "SubComment not added." });
        }


        public async Task<IActionResult> AddNotification(string NotificationText, string UserId, int? BlogId, string type)
        {
            var notification = new Notification
            {
                UserId = UserId,
                BlogId = BlogId,
                Type = type,
                NotificationText = NotificationText,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            _context.Add(notification);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Notification added successfully." });
        }
    }
}
