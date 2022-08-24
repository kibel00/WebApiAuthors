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
        [HttpGet("{id:int}", Name = "getBook")]
        public async Task<ActionResult<BookDTOWithAuthors>> Get(int id)
        {
            var book = await context.Book
                .Include(bookDb => bookDb.AuthorsBooks)
                .ThenInclude(authorBookDb => authorBookDb.Author).FirstOrDefaultAsync(x => x.Id == id);


            book.AuthorsBooks = book.AuthorsBooks.OrderBy(x => x.Order).ToList();
            return mapper.Map<BookDTOWithAuthors>(book);
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
            if (creationBookDTO.AuthorsId == null)
            {
                return BadRequest("Cannot create a book without authors");
            }
            var authorsId = await context.Authors
                .Where(authorDb => creationBookDTO.AuthorsId.Contains(authorDb.Id)).Select(x => x.Id).ToListAsync();

            if (creationBookDTO.AuthorsId.Count != authorsId.Count)
            {
                return BadRequest("there is no one of the authors sent");
            }

            var book = mapper.Map<Book>(creationBookDTO);
            AssignOrder(book);

            context.Add(book);
            await context.SaveChangesAsync();


            var bookDTO = mapper.Map<BookDTO>(book);
            return CreatedAtRoute("getBook", new { id = book.Id }, bookDTO);
        }

        private void AssignOrder(Book book)
        {
            if (book.AuthorsBooks != null)
            {
                for (int i = 0; i < book.AuthorsBooks.Count; i++)
                {
                    book.AuthorsBooks[i].Order = i;
                }
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Book>> Put(int id, CreationBookDTO creationBookDTO)
        {
            var bookDb = await context.Book.Include(x => x.AuthorsBooks).FirstOrDefaultAsync(x => x.Id == id);
            if (bookDb == null) return NotFound();


            bookDb = mapper.Map(creationBookDTO, bookDb);
            AssignOrder(bookDb);

            await context.SaveChangesAsync();
            return NoContent();

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
