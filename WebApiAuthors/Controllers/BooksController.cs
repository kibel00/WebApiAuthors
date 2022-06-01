using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public BooksController(ApplicationDbContext context)
        {
            this.context = context;
        }
        //[HttpGet("{id:int}")]
        //public async Task<ActionResult<Book>> Get(int id)
        //{
        //    var book = await context.Book.Include(x => x.Author).FirstOrDefaultAsync(x => x.Id == id);
        //    if (book == null) { return NotFound(); }
        //    return book;
        //}

        //[HttpGet]
        //public async Task<ActionResult<List<Book>>> Get()
        //{
        //    return await context.Book.Include(x => x.Author).ToListAsync();
        //}
        //[HttpPost]
        //public async Task<ActionResult> Post(Book book)
        //{
        //    var authorExist = await context.Book.Include(x => x.Author).AnyAsync(x => x.AuthorId == book.AuthorId);
        //    if (!authorExist) { return BadRequest($"No existe el autor del Id igual {book.AuthorId}"); }
        //    context.Add(book);
        //    await context.SaveChangesAsync();
        //    return Ok(book);
        //}

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Book>> Put(Book book, int id)
        {
            if (book.Id != id) { return BadRequest("El id del libro no coincide con el id de la url"); }
            var exist = await context.Book.AnyAsync(x => x.Id == id);
            if (!exist) { return NotFound(); }
            context.Update(book);
            await context.SaveChangesAsync();

            return Ok();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var book = await context.Book.FindAsync(id);
            if (book == null) { return NotFound(); }
            context.Remove(book);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
