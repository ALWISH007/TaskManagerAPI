using Microsoft.AspNetCore.Mvc;// brings ControllerBase, NotFound(), Ok()
using TaskManagerAPI.Models;
using TaskManagerAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace TaskManagerAPI.Controllers; 

[ApiController]//With [ApiController] you can also rely on DataAnnotations (e.g., [Required]) on TaskItem and let the framework auto-return 400 when ModelState is invalid. Also gets model TaskItem from request body
[Route("api/[controller]")] // The route will be /api/taskitems
[Authorize] // <<< ADD THIS LINE to protect the entire controller
//[Authorize(Roles = "Admin")] //Only users whose JWT contains a role claim with "Admin" can access this action.
public class TaskItemsController : ControllerBase
{
	private readonly ITaskService _taskService;// readonly means can not be reassigned later

	// Constructor Injection: The framework provides the ITaskService
	public TaskItemsController(ITaskService taskService)
	{
		_taskService = taskService;
	}

	// GET: api/taskitems
	[HttpGet]
	public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks() // use ActionResult when returning some content
	{
		var tasks = await _taskService.GetAllTasksAsync();
		return Ok(tasks); // HTTP 200 OK with the tasks list
	}
    //[Authorize(Policy = "MustBeOver18")] //You can use policies for more complex requirements:
    // GET: api/taskitems/5
    [HttpGet("{id}")]
	public async Task<ActionResult<TaskItem>> GetTask(int id) // [ApiController] binds {id} to parameter id
	{
		var task = await _taskService.GetTaskByIdAsync(id);

		if (task == null)
		{
			return NotFound(); // HTTP 404
		}

		return Ok(task); // HTTP 200 OK
	}

    //[AllowAnonymous] //If a controller has [Authorize], you can still allow specific actions to be accessed publicly:
    // POST: api/taskitems
    [HttpPost]
	public async Task<ActionResult<TaskItem>> PostTask(TaskItem taskItem)
	{
		// Basic validation
		if (string.IsNullOrEmpty(taskItem.Title))
		{
			return BadRequest("Title is required."); // HTTP 400
		}

		var createdTask = await _taskService.AddTaskAsync(taskItem);

		// Returns HTTP 201 Created, a location header, and the new object
		//According to REST conventions, POST should return 201 with a pointer to the newly created resource.
		return CreatedAtAction(
								nameof(GetTask),               // (1) Action name to generate URL , Safer automatically update (compiler-checked)
								new { id = createdTask.Id },   // (2) Route values (used in URL) ,Anonymous object with route values.
								createdTask                    // (3) Response body (the new object itself). The newly created object is returned in the response body, serialized to JSON.
							  );		

	}

	// PUT: api/taskitems/5
	[HttpPut("{id}")]
	public async Task<IActionResult> PutTask(int id, TaskItem taskItem)
	{
		if (id != taskItem.Id)
		{
			return BadRequest("ID in URL does not match ID in body."); // HTTP 400
		}

		var updatedTask = await _taskService.UpdateTaskAsync(id, taskItem);
		if (updatedTask == null)
		{
			return NotFound(); // HTTP 404
		}

		return NoContent(); // HTTP 204 No Content (standard response for PUT)
	}

	// DELETE: api/taskitems/5
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteTask(int id)
	{
		var wasDeleted = await _taskService.DeleteTaskAsync(id);
		if (!wasDeleted)
		{
			return NotFound(); // HTTP 404
		}

		return NoContent(); // HTTP 204 No Content
	}
}