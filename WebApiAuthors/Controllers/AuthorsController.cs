using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.DTOs;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly ILogger<AuthorsController> logger;
        private readonly IMapper mapper;

        public AuthorsController(ApplicationDbContext context, ILogger<AuthorsController> logger, IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        [HttpGet("Autores")]
        [HttpGet("/Autores")]
        [HttpGet]
        public async Task<ActionResult<List<AuthorDTO>>> Get()
        {
            logger.LogInformation("Getting authors");
            var authors = await context.Authors.ToListAsync();
            return mapper.Map<List<AuthorDTO>>(authors);
        }

        [HttpGet("{id:int}", Name = "getAuthors")]
        public async Task<ActionResult<AuthorDTOWithBook>> Get(int id)
        {
            var authors = await context.Authors
                .Include(authorBookDb => authorBookDb.AuthorsBooks)
                .ThenInclude(authorDb => authorDb.Book).SingleOrDefaultAsync(x => x.Id == id);
            if (authors == null)
            {
                return NotFound();
            }
            return mapper.Map<AuthorDTOWithBook>(authors);
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<List<AuthorDTO>>> Get(string name)
        {
            var author = await context.Authors.Where(x => x.Name.Contains(name)).ToListAsync();


            return mapper.Map<List<AuthorDTO>>(author);
        }



        [HttpPost]
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
            return CreatedAtRoute("getAuthors", new { id = authors.Id }, authorDTO);
        }

        [HttpPut("{id:int}")]
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

        [HttpDelete("{id:int}")]
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
