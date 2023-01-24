using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.DTOs;
using WebApiAuthors.Entities;
using WebApiAuthors.Utilities;

namespace WebApiAuthors.Controllers.V1
{
    [Route("api/v1/books/{bookId:int}/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public CommentsController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet(Name = "getCommentBook")]
        public async Task<ActionResult<List<CommentsDTO>>> Get(int bookId, [FromQuery] PaginationDTO paginationDTO)
        {
            var bookExist = await context.Book.AnyAsync(x => x.Id == bookId);
            if (bookExist)
            {
                var queryable = context.Comments.Where(x => x.BookId == bookId).AsQueryable();
                await HttpContext.InsertParamtersHeadPagination(queryable);
                var comment = await queryable.OrderBy(comment => comment.Id).Paginate(paginationDTO).ToListAsync();
                return mapper.Map<List<CommentsDTO>>(comment);
            }
            return NotFound();
        }

        [HttpGet("id:int", Name = "getComment")]
        public async Task<ActionResult<CommentsDTO>> GetById(int id)
        {
            var comment = await context.Comments.FirstOrDefaultAsync(x => x.Id == id);
            if (comment == null)
            {
                return NotFound();
            }
            return mapper.Map<CommentsDTO>(comment);
        }

        [HttpPost(Name = "createComment")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int bookId, CommentCreationDTO commentCreationDTO)
        {
            var emailClaim = HttpContext.User.Claims.Where(claims => claims.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var user = await userManager.FindByEmailAsync(email);
            var userId = user.Id;
            var existBook = await context.Book.AnyAsync(x => x.Id == bookId);
            if (!existBook)
            {
                return NotFound();
            }
            var comment = mapper.Map<Comment>(commentCreationDTO);
            comment.BookId = bookId;
            comment.UserId = userId;
            context.Add(comment);
            await context.SaveChangesAsync();
            var commentDTO = mapper.Map<CommentsDTO>(comment);
            return CreatedAtRoute("getComment", new { id = comment.Id, bookId }, commentDTO);
        }


        [HttpPut("{id:int}", Name = "updateComment")]
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
