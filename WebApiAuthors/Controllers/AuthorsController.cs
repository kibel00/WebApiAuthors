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

        public AuthorsController(ApplicationDbContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public async Task<ActionResult<List<Author>>> Get()
        {
            return await context.Authors.Include(x => x.Books).ToListAsync();
        }


        [HttpPost]
        public async Task<ActionResult> Post(Author author)
        {
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
