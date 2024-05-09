using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bislerium.Data;
using Bislerium.Models;

namespace Bislerium.Controllers
{
    public class BlogUpadateHistoriesController : Controller
    {
        private readonly BisleriumContext _context;

        public BlogUpadateHistoriesController(BisleriumContext context)
        {
            _context = context;
        }

        // GET: BlogUpadateHistories
        public async Task<IActionResult> Index()
        {
            var bisleriumContext = _context.BlogUpadateHistory.Include(b => b.Blog).Include(b => b.User);
            return View(await bisleriumContext.ToListAsync());
        }

        // GET: BlogUpadateHistories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogUpadateHistory = await _context.BlogUpadateHistory
                .Include(b => b.Blog)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogUpadateHistory == null)
            {
                return NotFound();
            }

            return View(blogUpadateHistory);
        }

        // GET: BlogUpadateHistories/Create
        public IActionResult Create()
        {
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: BlogUpadateHistories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BlogId,UserId,CreatedDate,UpdatedDate")] BlogUpadateHistory blogUpadateHistory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(blogUpadateHistory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", blogUpadateHistory.BlogId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", blogUpadateHistory.UserId);
            return View(blogUpadateHistory);
        }

        // GET: BlogUpadateHistories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogUpadateHistory = await _context.BlogUpadateHistory.FindAsync(id);
            if (blogUpadateHistory == null)
            {
                return NotFound();
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", blogUpadateHistory.BlogId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", blogUpadateHistory.UserId);
            return View(blogUpadateHistory);
        }

        // POST: BlogUpadateHistories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,UserId,CreatedDate,UpdatedDate")] BlogUpadateHistory blogUpadateHistory)
        {
            if (id != blogUpadateHistory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(blogUpadateHistory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogUpadateHistoryExists(blogUpadateHistory.Id))
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
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", blogUpadateHistory.BlogId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", blogUpadateHistory.UserId);
            return View(blogUpadateHistory);
        }

        // GET: BlogUpadateHistories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogUpadateHistory = await _context.BlogUpadateHistory
                .Include(b => b.Blog)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogUpadateHistory == null)
            {
                return NotFound();
            }

            return View(blogUpadateHistory);
        }

        // POST: BlogUpadateHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blogUpadateHistory = await _context.BlogUpadateHistory.FindAsync(id);
            if (blogUpadateHistory != null)
            {
                _context.BlogUpadateHistory.Remove(blogUpadateHistory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogUpadateHistoryExists(int id)
        {
            return _context.BlogUpadateHistory.Any(e => e.Id == id);
        }
    }
}
