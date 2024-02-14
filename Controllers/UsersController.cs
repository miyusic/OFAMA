using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using OFAMA.Models;
using System.Threading.Tasks;
using System.Net;
using System.ComponentModel.DataAnnotations;
using OFAMA.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNet.Identity;

namespace OFAMA.Controllers
{
    public class UsersController : Controller
    {
        private  ApplicationDbContext _context;

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
        
        /*
        public async Task<IActionResult>Edit(string id)
        {
            if (id == null) {
            return NotFound();
            }
            var target = await _context.ApplicationUser.FindAsync(id);

            if (target == null)
            {
                return NotFound();
            }
            UserEdit model = new UserEdit() { Email = target.Email };
            return View(model);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>Edit(string id, UserEdit model)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var target = await _context.ApplicationUser.FindAsync(id);
                target.Email=model.Email;
                target.UserName=model.UserName;
                target.Status=model.Status;
                target.Authority=model.Authority;
                var resultUpdate =_context.Update(target);
                
            }
            return View(model);
        }
        */
        
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
        public async Task<IActionResult> Edit(string id, [Bind("Id,UserName,Email,Status,Authority")]ApplicationUser applicationuser)
        {

            if (string.Compare(id,applicationuser.Id,true)!=0)
            {
                
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                Console.WriteLine("kokomadeOK");
                Console.WriteLine(id);
                Console.WriteLine(applicationuser.Id);
                var saved = false;
                try
                {

                    //applicationuser.Version = Guid.NewGuid();
                    _context.Update(applicationuser);

                    await _context.SaveChangesAsync();
                    saved = true;
                }
                catch(DbUpdateConcurrencyException ex)
                {
                    foreach(var entry in ex.Entries)
                    {
                        if(entry.Entity is ApplicationUser)
                        {
                            var proposedValues = entry.CurrentValues;
                            var databaseValues = entry.GetDatabaseValues();
                            Console.WriteLine("kokomadeOK");


                            foreach(var property in proposedValues.Properties)
                            {
                                var proposedValue = proposedValues[property];
                                var databaseValue = proposedValues[property];

                                proposedValues[property] = databaseValue;
                            }
                            entry.OriginalValues.SetValues(databaseValues);
                        }
                        else
                        {
                            throw new NotSupportedException(
                                "Don't know how to handle concurrency conflicts for");
                        }
                    }
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

