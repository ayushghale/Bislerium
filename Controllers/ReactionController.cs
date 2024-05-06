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
    public class ReactionController : Controller
    {
        private readonly BisleriumContext _context;

        public ReactionController(BisleriumContext context)
        {
            _context = context;
        }

        // GET: Reaction
        public async Task<IActionResult> Index()
        {
            var bisleriumContext = _context.Reaction.Include(r => r.Blog).Include(r => r.Comment).Include(r => r.User);
            return View(await bisleriumContext.ToListAsync());
        }

        // GET: Reaction/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reaction = await _context.Reaction
                .Include(r => r.Blog)
                .Include(r => r.Comment)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (reaction == null)
            {
                return NotFound();
            }

            return View(reaction);
        }

        // GET: Reaction/Create
        public IActionResult Create()
        {
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description");
            ViewData["commentId"] = new SelectList(_context.Comment, "Id", "CommentText");
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Reaction/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,BlogId,commentId,Upvote,Downvote")] Reaction reaction)
        {

            reaction.UserID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            reaction.CreatedDate = DateTime.Now;
            reaction.UpdatedDate = DateTime.Now;



            Console.WriteLine("================================================================");
            Console.WriteLine(reaction.UserID);
            Console.WriteLine(reaction.commentId);
            Console.WriteLine(reaction.Upvote);
            Console.WriteLine(reaction.Downvote);
            Console.WriteLine(reaction.CreatedDate);
            Console.WriteLine(reaction.UpdatedDate);
            _context.Add(reaction);
            await _context.SaveChangesAsync();
            Console.WriteLine("================================================================");
            if (ModelState.IsValid)
            {
                _context.Add(reaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", reaction.BlogId);
            ViewData["commentId"] = new SelectList(_context.Comment, "Id", "CommentText", reaction.commentId);
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", reaction.UserID);
            return View(reaction);
        }

        // GET: Reaction/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reaction = await _context.Reaction.FindAsync(id);
            if (reaction == null)
            {
                return NotFound();
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", reaction.BlogId);
            ViewData["commentId"] = new SelectList(_context.Comment, "Id", "CommentText", reaction.commentId);
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", reaction.UserID);
            return View(reaction);
        }

        // POST: Reaction/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,BlogId,commentId,UserID,Upvote,Downvote,CreatedDate,UpdatedDate")] Reaction reaction)
        {
            if (id != reaction.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReactionExists(reaction.ID))
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
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", reaction.BlogId);
            ViewData["commentId"] = new SelectList(_context.Comment, "Id", "CommentText", reaction.commentId);
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", reaction.UserID);
            return View(reaction);
        }

        // GET: Reaction/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reaction = await _context.Reaction
                .Include(r => r.Blog)
                .Include(r => r.Comment)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (reaction == null)
            {
                return NotFound();
            }

            return View(reaction);
        }

        // POST: Reaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reaction = await _context.Reaction.FindAsync(id);
            if (reaction != null)
            {
                _context.Reaction.Remove(reaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReactionExists(int id)
        {
            return _context.Reaction.Any(e => e.ID == id);
        }
    }
}
