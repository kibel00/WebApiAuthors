using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.DTOs;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers
{
    [Route("api/books/{bookId:int}/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public CommentsController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<CommentsDTO>>> Get(int bookId)
        {
            var bookExist = await context.Book.AnyAsync(x => x.Id == bookId);
            if (bookExist)
            {
                var comment = await context.Comments.Where(x => x.BookId == bookId).ToListAsync();
                return mapper.Map<List<CommentsDTO>>(comment);
            }
            return NotFound();
        }

        [HttpGet("id:int", Name = "GetComment")]
        public async Task<ActionResult<CommentsDTO>> GetById(int id)
        {
            var comment = await context.Comments.FirstOrDefaultAsync(x => x.Id == id);
            if (comment == null)
            {
                return NotFound();
            }
            return mapper.Map<CommentsDTO>(comment);
        }

        [HttpPost]
        public async Task<ActionResult> Post(int bookId, CommentCreationDTO commentCreationDTO)
        {
            var existBook = await context.Book.AnyAsync(x => x.Id == bookId);
            if (!existBook)
            {
                return NotFound();
            }
            var comment = mapper.Map<Comment>(commentCreationDTO);
            comment.BookId = bookId;
            context.Add(comment);
            await context.SaveChangesAsync();
            var commentDTO = mapper.Map<CommentsDTO>(comment);
            return CreatedAtRoute("GetComment", new { id = comment.Id, bookId = bookId }, commentDTO);
        }


        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int bookId, int id, CommentCreationDTO commentCreationDTO)
        {
            var bookExist = await context.Book.AnyAsync(x => x.Id == bookId);
            if (!bookExist)
            {
                return NotFound();
            }

            var commentExit = await context.Comments.AnyAsync(x => x.Id == id);
            if (!commentExit) return NotFound();

            var comment = mapper.Map<Comment>(commentCreationDTO);
            comment.Id = id;
            comment.BookId = bookId;
            context.Update(comment);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
