using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.DTOs;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public BooksController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookDTO>> Get(int id)
        {
            var book = await context.Book.FirstOrDefaultAsync(x => x.Id == id);
            //if (book == null) { return NotFound(); }
            return mapper.Map<BookDTO>(book);
        }

        [HttpGet]
        public async Task<ActionResult<List<BookDTO>>> Get()
        {
            var book = await context.Book.ToListAsync();
            return mapper.Map<List<BookDTO>>(book);
        }
        [HttpPost]
        public async Task<ActionResult> Post(CreationBookDTO creationBookDTO)
        {
            //var authorExist = await context.Book.Include(x => x.Author).AnyAsync(x => x.AuthorId == book.AuthorId);
            //if (!authorExist) { return BadRequest($"No existe el autor del Id igual {book.AuthorId}"); }
            var book = mapper.Map<Book>(creationBookDTO);
            context.Add(book);
            await context.SaveChangesAsync();
            return Ok(book);
        }

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
