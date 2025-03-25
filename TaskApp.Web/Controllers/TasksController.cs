using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskApp.Data;
using TaskApp.Data.Models;
using TaskApp.Web.Models;

namespace TaskApp.Web.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public TasksController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Tasks
        public async Task<IActionResult> Index(string searchString)
        {
            var tasksQuery = _context.Tasks
                .Include(t => t.Assignee)
                .AsQueryable();
            
            // Apply search filter if provided
            if (!string.IsNullOrEmpty(searchString))
            {
                tasksQuery = tasksQuery.Where(t => t.Title.Contains(searchString) || 
                                                 t.Description.Contains(searchString));
                ViewData["CurrentFilter"] = searchString;
            }
            
            var tasks = await tasksQuery.ToListAsync();

            var taskViewModels = tasks.Select(t => new TaskViewModel
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                DueDate = t.DueDate,
                Difficulty = t.Difficulty,
                IsCompleted = t.IsCompleted,
                UserId = t.UserId,
                AssigneeName = t.Assignee?.FullName ?? "Unassigned"
            }).ToList();

            return View(new TaskListViewModel { Tasks = taskViewModels });
        }

        // GET: Tasks/Create
        public async Task<IActionResult> Create()
        {
            var users = await _userManager.Users.ToListAsync();
            var viewModel = new TaskViewModel
            {
                AssigneeList = new SelectList(users, "Id", "FullName")
            };
            return View(viewModel);
        }

        // POST: Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskViewModel model)
        {
            if (ModelState.IsValid)
            {
                var todoTask = new TodoTask
                {
                    Title = model.Title,
                    Description = model.Description,
                    DueDate = model.DueDate,
                    Difficulty = model.Difficulty,
                    UserId = model.UserId,
                    IsCompleted = false // Novas tarefas são não concluídas por padrão
                };

                _context.Add(todoTask);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Se chegamos até aqui, algo falhou, reexibir formulário
            var users = await _userManager.Users.ToListAsync();
            model.AssigneeList = new SelectList(users, "Id", "FullName", model.UserId);
            return View(model);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todoTask = await _context.Tasks
                .Include(t => t.Assignee)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (todoTask == null)
            {
                return NotFound();
            }

            var users = await _userManager.Users.ToListAsync();
            var viewModel = new TaskViewModel
            {
                Id = todoTask.Id,
                Title = todoTask.Title,
                Description = todoTask.Description,
                DueDate = todoTask.DueDate,
                Difficulty = todoTask.Difficulty,
                UserId = todoTask.UserId,
                IsCompleted = todoTask.IsCompleted,
                AssigneeList = new SelectList(users, "Id", "FullName", todoTask.UserId)
            };

            return View(viewModel);
        }

        // POST: Tasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var todoTask = await _context.Tasks.FindAsync(id);
                    if (todoTask == null)
                    {
                        return NotFound();
                    }

                    todoTask.Title = model.Title;
                    todoTask.Description = model.Description;
                    todoTask.DueDate = model.DueDate;
                    todoTask.Difficulty = model.Difficulty;
                    todoTask.UserId = model.UserId;
                    todoTask.IsCompleted = model.IsCompleted;

                    _context.Update(todoTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TodoTaskExists(id))
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

            var users = await _userManager.Users.ToListAsync();
            model.AssigneeList = new SelectList(users, "Id", "FullName", model.UserId);
            return View(model);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var todoTask = await _context.Tasks.FindAsync(id);
            if (todoTask != null)
            {
                _context.Tasks.Remove(todoTask);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Tasks/ToggleComplete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleComplete(int id)
        {
            var todoTask = await _context.Tasks.FindAsync(id);
            if (todoTask != null)
            {
                todoTask.IsCompleted = !todoTask.IsCompleted;
                _context.Update(todoTask);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TodoTaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
} 