using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly ILogger<AuthorsController> logger;

        public AuthorsController(ApplicationDbContext context, ILogger<AuthorsController> logger)
        {
            this.context = context;
            this.logger = logger;
        }
        [HttpGet("Autores")]
        [HttpGet("/Autores")]
        [HttpGet]
        public async Task<ActionResult<List<Author>>> Get()
        {
            logger.LogInformation("Getting authors");
            return await context.Authors.Include(x => x.Books).ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Author>> Get(int id)
        {
            var author = await context.Authors.Include(x => x.Books).SingleOrDefaultAsync(x => x.Id == id);
            if (author == null)
            {
                return NotFound();
            }
            return author;
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<Author>> Get(string name)
        {
            var author = await context.Authors.Include(x => x.Books).SingleOrDefaultAsync(x => x.Name.Contains(name));
            if (author == null) { return NotFound(); }
            return author;
        }



        [HttpPost]
        public async Task<ActionResult> Post(Author author)
        {
            var exist = await context.Authors.AnyAsync(x => x.Name == author.Name);
            if (exist)
            {
                return BadRequest($"this author {author.Name} already exist");
            }
            context.Add(author);
            await context.SaveChangesAsync();
            return Ok(author);
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
