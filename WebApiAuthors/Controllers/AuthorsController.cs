using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.DTOs;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers
{
    [Route("api/[controller]")]
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

        [HttpGet(Name = "getAuthors")]
        [AllowAnonymous]
        public async Task<ActionResult<CollectionResources<AuthorDTO>>> Get()
        {
            logger.LogInformation("Getting authors");
            var authors = await context.Authors.ToListAsync();
            var dto = mapper.Map<List<AuthorDTO>>(authors);


            var isAdmin = await authorizationService.AuthorizeAsync(User, "isAdmin");



            dto.ForEach(x => LinkGenerate(x, isAdmin.Succeeded));

            var result = new CollectionResources<AuthorDTO>() { Values = dto };
            result.Links.Add(new HateOASData(
                link: Url.Link("getAuthors", new { }),
                description: "self",
                method: "GET"));


            if (isAdmin.Succeeded)
            {
                result.Links.Add(new HateOASData(
                    link: Url.Link("authorCreate", new { }),
                    description: "author-create",
                    method: "POST"));
            }

            return result;
        }

        [HttpGet("{id:int}", Name = "getAuthors")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthorDTOWithBook>> Get(int id)
        {
            var authors = await context.Authors
                .Include(authorBookDb => authorBookDb.AuthorsBooks)
                .ThenInclude(authorDb => authorDb.Book).SingleOrDefaultAsync(x => x.Id == id);
            if (authors == null)
            {
                return NotFound();
            }
            var isAdmin = await authorizationService.AuthorizeAsync(User, "isAdmin");


            var dto = mapper.Map<AuthorDTOWithBook>(authors);
            LinkGenerate(dto, isAdmin.Succeeded);
            return dto;
        }



        private void LinkGenerate(AuthorDTO authorDTO, bool isAdmin)
        {
            authorDTO.Links.Add(new HateOASData(link: Url.Link("getAuthors", new { id = authorDTO.Id }),
                                                description: "self",
                                                method: "GET"));


            if (isAdmin)
            {
                authorDTO.Links.Add(new HateOASData(link: Url.Link("updateAuthor", new { id = authorDTO.Id }),
                                                   description: "update-author",
                                                   method: "PUT"));


                authorDTO.Links.Add(new HateOASData(link: Url.Link("deleteAuthor", new { id = authorDTO.Id }),
                                                   description: "author-delete",
                                                   method: "DELETE"));
            }

        }

        [HttpGet("{name}", Name = "getAuthorsByName")]
        public async Task<ActionResult<List<AuthorDTO>>> Get(string name)
        {
            var author = await context.Authors.Where(x => x.Name.Contains(name)).ToListAsync();


            return mapper.Map<List<AuthorDTO>>(author);
        }



        [HttpPost(Name = "authorCreate")]
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

        [HttpPut("{id:int}", Name = "updateAuthor")]
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

        [HttpDelete("{id:int}", Name = "deleteAuthor")]
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
