using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bislerium.Data;
using Bislerium.Models;
using System.Security.Claims;

namespace Bislerium.Controllers
{
    public class BlogsController : Controller
    {
        private readonly BisleriumContext _context;
        private static string _UserDashboard = "~/Views/User/Dashboard.cshtml";

        public BlogsController(BisleriumContext context)
        {
            _context = context;
        }


        // GET: Blogs
        public async Task<IActionResult> Index()
        {
            var bisleriumContext = _context.Blogs.Include(b => b.User);
            return View(await bisleriumContext.ToListAsync());
        }

        // GET: Blogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogs = await _context.Blogs
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogs == null)
            {
                return NotFound();
            }

            return View(blogs);
        }

        // GET: Blogs/Create
        public IActionResult Create()
        {
            ViewData["userId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Heading,Description")] Blogs blogs, IFormFile Image)
        {


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
                        return View(blogs);
                    }

                    // Process the uploaded image, such as saving it to the file system or database
                    // For simplicity, let's assume you have a method to handle saving the image
                    blogs.Image = await SaveImageAsync(Image);
                }
                else
                {

                    blogs.Image = "drxgrddrcx";
                }

                blogs.UpdatedDate = DateTime.Now;
                blogs.CreatedDate = DateTime.Now;
                _context.Add(blogs);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["userId"] = new SelectList(_context.Users, "Id", "Id", blogs.userId);
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> SaveImageAsync(IFormFile Image)
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

        // GET: Blogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogs = await _context.Blogs.FindAsync(id);
            if (blogs == null)
            {
                return NotFound();
            }
            ViewData["userId"] = new SelectList(_context.Users, "Id", "Id", blogs.userId);
            return View(blogs);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Heading,Description")] Blogs blogs, IFormFile Image)
        {
            if (id != blogs.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Assign the current user's ID to the UserId property of the post
                blogs.userId = userId;
                try
                {
                    blogs.UpdatedDate = DateTime.Now;
                    blogs.CreatedDate = blogs.CreatedDate;

                    if (Image != null)
                    {
                        // Check if the file size exceeds the limit (3 MB)
                        if (Image.Length > 3 * 1024 * 1024) // 3 MB in bytes
                        {
                            ModelState.AddModelError("Image", "The image must be less than 3 Megabytes (MB).");
                            return View(blogs);
                        }

                        // Process the uploaded image, such as saving it to the file system or database
                        // For simplicity, let's assume you have a method to handle saving the image
                        blogs.Image = await SaveImageAsync(Image);
                    }

                    _context.Update(blogs);
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["userId"] = new SelectList(_context.Users, "Id", "Id", blogs.userId);
            return View(blogs);
        }

        // GET: Blogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

            return RedirectToAction(nameof(Index));
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blogs = await _context.Blogs.FindAsync(id);
            if (blogs != null)
            {
                _context.Blogs.Remove(blogs);
            }


            return RedirectToAction(nameof(Index));
        }

        private bool BlogsExists(int id)
        {
            return _context.Blogs.Any(e => e.Id == id);
        }
    }
}
