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

        [HttpPost]
        public async Task<ActionResult> Post(int bookId, CommentCreationDTO commentCreationDTO)
        {
            var existBook = await context.Book.AnyAsync(x => x.Id == bookId);
            if (existBook)
            {
                var comment = mapper.Map<Comment>(commentCreationDTO);
                comment.BookId = bookId;
                context.Add(comment);
                await context.SaveChangesAsync();
                return Ok(comment);
            }
            return NotFound();
        }
    }
}
