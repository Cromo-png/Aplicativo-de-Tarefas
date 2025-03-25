using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TaskApp.Data.Models;

namespace TaskApp.Web.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // GET: Users
        public async Task<IActionResult> Index(string? searchString = null)
        {
            var usersQueryable = _userManager.Users.AsQueryable();
            
            if (!string.IsNullOrEmpty(searchString))
            {
                usersQueryable = usersQueryable.Where(u => 
                    (u.UserName != null && u.UserName.Contains(searchString)) || 
                    (u.Email != null && u.Email.Contains(searchString)) ||
                    (u.FullName != null && u.FullName.Contains(searchString)));
                ViewData["CurrentFilter"] = searchString;
            }
            
            return View(await usersQueryable.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
    }
} 