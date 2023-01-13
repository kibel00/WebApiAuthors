using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.DTOs;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers.V1
{
    [Route("api/v1/[controller]")]
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
        [HttpGet("{id:int}", Name = "getBookById")]
        public async Task<ActionResult<BookDTOWithAuthors>> Get(int id)
        {
            var book = await context.Book
                .Include(bookDb => bookDb.AuthorsBooks)
                .ThenInclude(authorBookDb => authorBookDb.Author).FirstOrDefaultAsync(x => x.Id == id);

            if (book == null) return NotFound();


            book.AuthorsBooks = book.AuthorsBooks.OrderBy(x => x.Order).ToList();
            return mapper.Map<BookDTOWithAuthors>(book);
        }

        [HttpGet(Name = "getbook")]
        public async Task<ActionResult<List<BookDTO>>> Get()
        {
            var book = await context.Book.ToListAsync();
            return mapper.Map<List<BookDTO>>(book);
        }


        [HttpPost(Name = "createBook")]
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
            return CreatedAtRoute("getBookById", new { id = book.Id }, bookDTO);
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

        [HttpPut("{id:int}", Name = "updateBook")]
        public async Task<ActionResult<Book>> Put(int id, CreationBookDTO creationBookDTO)
        {
            var bookDb = await context.Book.Include(x => x.AuthorsBooks).FirstOrDefaultAsync(x => x.Id == id);
            if (bookDb == null) return NotFound();


            bookDb = mapper.Map(creationBookDTO, bookDb);
            AssignOrder(bookDb);

            await context.SaveChangesAsync();
            return NoContent();

        }


        [HttpDelete("{id:int}", Name = "deleteBook")]
        public async Task<ActionResult> Delete(int id)
        {
            var book = await context.Book.AnyAsync(x => x.Id == id);
            if (!book) { return NotFound(); }
            context.Remove(new Book() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }



        [HttpPatch("{id:int}", Name = "patchBook")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<BookPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var bookDb = await context.Book.FirstOrDefaultAsync(x => x.Id == id);
            if (bookDb == null)
            {
                return NotFound();
            }

            var bookDto = mapper.Map<BookPatchDTO>(bookDb);

            patchDocument.ApplyTo(bookDto, ModelState);

            var isValid = TryValidateModel(bookDto);

            if (!isValid)
            {
                return BadRequest(ModelState);
            }


            mapper.Map(bookDto, bookDb);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
