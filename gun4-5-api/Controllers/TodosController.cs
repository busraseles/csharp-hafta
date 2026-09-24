using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace gun4_5_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly ITodoRepository _repo;
    private readonly ILogger<TodosController> _logger;

    public TodosController(ITodoRepository repo, ILogger<TodosController> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<TodoResponse>> GetAll()
        => Ok(_repo.GetAll().Select(TodoResponse.From));

    [HttpGet("{id:int}")]
    public ActionResult<TodoResponse> Get(int id)
    {
        var item = _repo.GetById(id);
        if (item is null)
            return NotFound();

        return Ok(TodoResponse.From(item));
    }

    [HttpPost]
    public ActionResult<TodoResponse> Create(CreateTodoRequest request)
    {
        var item = _repo.Add(request.Title);

        // 21. Adım: Yapılandırılmış log satırı
        _logger.LogInformation("Görev eklendi: {Id} {Title}", item.Id, item.Title);

        return CreatedAtAction(nameof(Get), new { id = item.Id }, TodoResponse.From(item));
    }

    // PUT api/todos/{id}/complete: Görev tamamlama
    [HttpPut("{id:int}/complete")]
    public IActionResult Complete(int id)
    {
        var success = _repo.Complete(id);
        if (!success)
            return NotFound(); // 404

        return NoContent(); // 204 No Content
    }

    // DELETE api/todos/{id}: Görev silme
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var success = _repo.Remove(id);
        if (!success)
            return NotFound(); // 404

        return NoContent(); // 204 No Content
    }
}

public record CreateTodoRequest([Required, MaxLength(100)] string Title);

public record TodoResponse(int Id, string Title, bool IsDone)
{
    public static TodoResponse From(TodoItem t) => new(t.Id, t.Title, t.IsDone);
}