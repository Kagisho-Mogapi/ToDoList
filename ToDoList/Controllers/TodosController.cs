using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        private readonly TodoDbContext _context;

        public TodosController(TodoDbContext context)
        {
            _context = context;
        }

        [HttpGet("All")]
        public async Task<IEnumerable<Todo>> GetAll()
        {
            return await _context.Todos.ToListAsync();
        }

        [HttpGet("Get By Id")]
        public async Task<IActionResult> GetById(int id)
        {
            var todo = await _context.Todos.FindAsync(id);

            if(todo == null)
                return NotFound();

            return Ok(todo);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(Todo todo)
        {
            await _context.Todos.AddAsync(todo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Create), todo);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update(int id,Todo todo)
        {
            if (todo.Id != id)
                return BadRequest();

            _context.Entry(todo).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var todo = _context.Todos.Find(id);

            if (todo == null)
                return NotFound();

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
