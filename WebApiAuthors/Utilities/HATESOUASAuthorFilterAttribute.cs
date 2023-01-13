using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApiAuthors.DTOs;
using WebApiAuthors.Services;

namespace WebApiAuthors.Utilities
{
    public class HATESOUASAuthorFilterAttribute : HatesOASFilterAttribute
    {
        private readonly LinksGenerator linkGenerator;

        public HATESOUASAuthorFilterAttribute(LinksGenerator linkGenerator)
        {
            this.linkGenerator = linkGenerator;
        }
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var shoudInclude = ShoudIncludeHateOAS(context);

            if (!shoudInclude)
            {
                await next();
                return;
            }

            var result = context.Result as ObjectResult;


            var authorDto = result.Value as AuthorDTO;
            if (authorDto == null)
            {
                var authorsDTO = result.Value as List<AuthorDTO> ?? throw new ArgumentNullException("an authorDTO or List<autorsDTO> instance was expected");

                authorsDTO.ForEach(async authors => await linkGenerator.LinkGenerate(authors));
                result.Value = authorsDTO;
            }
            else
            {
                await linkGenerator.LinkGenerate(authorDto);
            }

            await next();
        }
    }
}
