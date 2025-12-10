using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TaskTracker.Models;
using TaskTracker.Repositories;


namespace TaskTracker.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ITaskRepository _taskRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public TasksController(ITaskRepository taskRepository, UserManager<IdentityUser> userManager)
        {
            _taskRepository = taskRepository;
            _userManager = userManager;
        }

        private string GetUserId()
        {
            return _userManager.GetUserId(User)!;
        }

        // GET: /Tasks
        public async Task<IActionResult> Index(string? sortOrder)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["DueDateSortParm"] = string.IsNullOrEmpty(sortOrder) ? "due_desc" : "";
            ViewData["PrioritySortParm"] = sortOrder == "priority" ? "priority_desc" : "priority";
            ViewData["DoneSortParm"] = sortOrder == "done" ? "done_desc" : "done";

            var userId = GetUserId();
            var tasks = await _taskRepository.GetTasksForUserAsync(userId);

            // default sort: by due date ascending
            var ordered = sortOrder switch
            {
                "due_desc" => tasks.OrderByDescending(t => t.DueDate ?? DateTime.MaxValue),
                "priority" => tasks.OrderBy(t => t.Priority),
                "priority_desc" => tasks.OrderByDescending(t => t.Priority),
                "done" => tasks.OrderBy(t => t.IsDone),
                "done_desc" => tasks.OrderByDescending(t => t.IsDone),
                _ => tasks.OrderBy(t => t.DueDate ?? DateTime.MaxValue),
            };

            return View(ordered.ToList());
        }

        // GET: /Tasks/Create
        public IActionResult Create()
        {
            var task = new TaskItem
            {
                DueDate = DateTime.Today
            };
            return View(task);
        }

        // POST: /Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskItem task)
        {
            if (!ModelState.IsValid)
            {
                return View(task);
            }

            task.UserId = GetUserId();
            task.IsDone = false;

            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Tasks/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var userId = GetUserId();
            var task = await _taskRepository.GetByIdAsync(id, userId);

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: /Tasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskItem formTask)
        {
            if (id != formTask.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(formTask);
            }

            var userId = GetUserId();
            var task = await _taskRepository.GetByIdAsync(id, userId);

            if (task == null)
            {
                return NotFound();
            }

            // update allowed fields
            task.Title = formTask.Title;
            task.DueDate = formTask.DueDate;
            task.Priority = formTask.Priority;
            task.IsDone = formTask.IsDone;

            await _taskRepository.UpdateAsync(task);
            await _taskRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: /Tasks/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var task = await _taskRepository.GetByIdAsync(id, userId);

            if (task == null)
            {
                return NotFound();
            }

            await _taskRepository.DeleteAsync(task);
            await _taskRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: /Tasks/ToggleDone/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleDone(int id)
        {
            var userId = GetUserId();
            var task = await _taskRepository.GetByIdAsync(id, userId);

            if (task == null)
            {
                return NotFound();
            }

            task.IsDone = !task.IsDone;

            await _taskRepository.UpdateAsync(task);
            await _taskRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
