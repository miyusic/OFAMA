using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using OFAMA.Models;
using System.Threading.Tasks;
using System.Net;
using System.ComponentModel.DataAnnotations;
using OFAMA.Data;
using Microsoft.EntityFrameworkCore;

namespace OFAMA.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        //1.10追加
        //Get
        public async Task<IActionResult> Index()
        {
            return _context.ApplicationUser != null ?
                View(await _context.ApplicationUser.ToListAsync()) :
                Problem("Entity set 'ApplicationUserContext' is null");
        }

        //Get:Users/Details/5
        //もともとはDetails(int ? id)だった
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null || _context.ApplicationUser == null)
            {
                return NotFound();
            }
            var user = await _context.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        //Get:USers/Create
        public IActionResult Create()
        {
            return View();
        }

        //POST:Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserName,Email,Status,Authority")]ApplicationUser applicationuser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(applicationuser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            return View(applicationuser);
        }
        
        //Get: Users/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id ==null || _context.ApplicationUser == null)
            {
                return NotFound();
            }
            var applicationuser = await _context.ApplicationUser.FindAsync(id);
            if(applicationuser == null)
            {
                return NotFound();
            }
            return View(applicationuser);
        }
        //Post:Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("UserName,Email,Status,Authority")] ApplicationUser applicationuser)
        {
            if (id !=applicationuser.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(applicationuser);
                    await _context.SaveChangesAsync();
                }
                catch(DbUpdateConcurrencyException)
                {
                    if(!ApplicationUserExists(applicationuser.Id))
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
            return View(applicationuser);
        }

        //Get:Users/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if(id == null || _context.ApplicationUser == null)
            {
                return NotFound();
            }
            var applictationuser = await _context.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id);
            if (applictationuser == null)
            {
                return NotFound();
            }
            return View(applictationuser);
        }

        //POST:Users/Delete/5
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DelteConfirmed(string id)
        {
            if (_context.ApplicationUser == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ApplicationUsers' is null");
            }
            var applicationuser = await _context.ApplicationUser.FindAsync(id);
            if(applicationuser !=null)
            {
                _context.ApplicationUser.Remove(applicationuser);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApplicationUserExists(string id)
        {
            return (_context.ApplicationUser?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

