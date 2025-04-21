using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly StoreContext _storeContext;

        public TasksController(StoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        // GET: api/Tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TableTasks>>> GetAllTasks()
        {
            return await _storeContext.TableTasks
                .Include(t => t.User)
                .ToListAsync();
        }

        // GET: api/Tasks/user/5
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<TableTasks>>> GetTasksForUser(int userId)
        {
            var userExists = await _storeContext.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
                return NotFound("User not found");

            var tasks = await _storeContext.TableTasks
                .Where(t => t.UserId == userId)
                .ToListAsync();

            return Ok(tasks);
        }

        // GET: api/Tasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TableTasks>> GetTask(int id)
        {
            var task = await _storeContext.TableTasks
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return NotFound("Task not found");

            return Ok(task);
        }

        // POST: api/Tasks
        [HttpPost]
        public async Task<ActionResult<TableTasks>> CreateTask(TableTasks task)
        {
            // Validate task data
            if (string.IsNullOrEmpty(task.Title))
                return BadRequest("Task title is required");

            var userExists = await _storeContext.Users.AnyAsync(u => u.Id == task.UserId);
            if (!userExists)
                return BadRequest("Invalid UserId - User does not exist");

            _storeContext.TableTasks.Add(task);
            await _storeContext.SaveChangesAsync();

            // Load the User for the created task
            await _storeContext.Entry(task).Reference(t => t.User).LoadAsync();

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        // PUT: api/Tasks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TableTasks updatedTask)
        {
            if (id != updatedTask.Id)
                return BadRequest("Task ID mismatch");

            var existingTask = await _storeContext.TableTasks.FindAsync(id);
            if (existingTask == null)
                return NotFound("Task not found");

            // Validate task data
            if (string.IsNullOrEmpty(updatedTask.Title))
                return BadRequest("Task title is required");

            if (!await _storeContext.Users.AnyAsync(u => u.Id == updatedTask.UserId))
                return BadRequest("Invalid UserId - User does not exist");

            // Update only specific properties
            existingTask.Title = updatedTask.Title;
            existingTask.Content = updatedTask.Content;
            existingTask.IsCompleted = updatedTask.IsCompleted;
            existingTask.UserId = updatedTask.UserId;

            await _storeContext.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _storeContext.TableTasks.FindAsync(id);
            if (task == null)
                return NotFound("Task not found");

            _storeContext.TableTasks.Remove(task);
            await _storeContext.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/Tasks/5/complete
        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> CompleteTask(int id)
        {
            var task = await _storeContext.TableTasks.FindAsync(id);
            if (task == null)
                return NotFound("Task not found");

            task.IsCompleted = true;
            await _storeContext.SaveChangesAsync();

            return NoContent();
        }
    }
}