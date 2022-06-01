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
        public async Task<ActionResult<List<Author>>> Get()
        {
            logger.LogInformation("Getting authors");
            return await context.Authors.ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Author>> Get(int id)
        {
            var author = await context.Authors.SingleOrDefaultAsync(x => x.Id == id);
            if (author == null)
            {
                return NotFound();
            }
            return author;
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<Author>> Get(string name)
        {
            var author = await context.Authors.SingleOrDefaultAsync(x => x.Name.Contains(name));
            if (author == null) { return NotFound(); }
            return author;
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
            return Ok(creationAuthorDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Author>> Put(Author author, int id)
        {
            if (author.Id != id) { return BadRequest("El id del autor no coincide con el id de la url"); }
            var authorAuthor = await context.Authors.FindAsync(id);
            if (authorAuthor == null) { return NotFound(); }
            context.Update(author);
            await context.SaveChangesAsync();
            return Ok();
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
