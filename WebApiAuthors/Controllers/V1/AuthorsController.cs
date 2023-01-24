using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.DTOs;
using WebApiAuthors.Entities;
using WebApiAuthors.Utilities;

namespace WebApiAuthors.Controllers.V1
{
    [Route("api/[controller]")]
    //[Route("api/v1/[controller]")]
    [HeadIsPresentedInAttribute("x-version","1")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
    public class AuthorsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly ILogger<AuthorsController> logger;
        private readonly IMapper mapper;
        private readonly IAuthorizationService authorizationService;

        public AuthorsController(ApplicationDbContext context, ILogger<AuthorsController> logger, IMapper mapper, IAuthorizationService authorizationService)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "getAuthorsv1")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATESOUASAuthorFilterAttribute))]
        public async Task<ActionResult<List<AuthorDTO>>> Get()
        {
            logger.LogInformation("Getting authors");
            var authors = await context.Authors.ToListAsync();
            return mapper.Map<List<AuthorDTO>>(authors);
        }

        [HttpGet("{id:int}", Name = "getAuthorv1")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATESOUASAuthorFilterAttribute))]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<AuthorDTOWithBook>> Get(int id)
        {
            var authors = await context.Authors
                .Include(authorBookDb => authorBookDb.AuthorsBooks)
                .ThenInclude(authorDb => authorDb.Book).SingleOrDefaultAsync(x => x.Id == id);
            if (authors == null)
            {
                return NotFound();
            }


            var dto = mapper.Map<AuthorDTOWithBook>(authors);
            return dto;
        }

        [HttpGet("{name}", Name = "getAuthorsByNamev1")]
        public async Task<ActionResult<List<AuthorDTO>>> GetByName(string name)
        {
            var author = await context.Authors.Where(x => x.Name.Contains(name)).ToListAsync();


            return mapper.Map<List<AuthorDTO>>(author);
        }



        [HttpPost(Name = "authorCreatev1")]
        public async Task<ActionResult> Post(CreationAuthorDTO creationAuthorDTO)
        {
            var exist = await context.Authors.AnyAsync(x => x.Name == creationAuthorDTO.Name);
            if (exist)
            {
                return BadRequest($"this author {creationAuthorDTO.Name} already exist");
            }
            var authors = mapper.Map<Author>(creationAuthorDTO);
            context.Add(authors);
            await context.SaveChangesAsync();


            var authorDTO = mapper.Map<AuthorDTO>(authors);
            return CreatedAtRoute("getAuthorsv1", new { id = authors.Id }, authorDTO);
        }

        [HttpPut("{id:int}", Name = "updateAuthorv1")]
        public async Task<ActionResult<Author>> Put(CreationAuthorDTO creationAuthorDTO, int id)
        {

            var authorAuthor = await context.Authors.FindAsync(id);
            if (authorAuthor == null) { return NotFound(); }

            var authors = mapper.Map<Author>(creationAuthorDTO);
            authors.Id = id;
            context.Update(authors);
            await context.SaveChangesAsync();
            return NoContent();
        }


        /// <summary>
        /// Delete a author
        /// </summary>
        /// <param name="id">authors id to delete</param>
        /// <returns></returns>
        [HttpDelete("{id:int}", Name = "deleteAuthorv1")]
        public async Task<ActionResult> Delete(int id)
        {
            var author = await context.Authors.FindAsync(id);
            if (author == null) { return NotFound(); }
            context.Remove(author);
            await context.SaveChangesAsync();
            return Ok();
        }

    }
}
