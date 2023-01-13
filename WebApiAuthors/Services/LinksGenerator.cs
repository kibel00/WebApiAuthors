using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using WebApiAuthors.DTOs;

namespace WebApiAuthors.Services
{
    public class LinksGenerator
    {
        private readonly IAuthorizationService authorizationService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IActionContextAccessor actionContextAccessor;

        public LinksGenerator(IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor, IActionContextAccessor actionContextAccessor)
        {
            this.authorizationService = authorizationService;
            this.httpContextAccessor = httpContextAccessor;
            this.actionContextAccessor = actionContextAccessor;
        }

        private IUrlHelper BuildUrlHelper()
        {
            var factory = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            return factory.GetUrlHelper(actionContextAccessor.ActionContext);
        }
        private async Task<bool> IsAdmin()
        {
            var httpContext = httpContextAccessor.HttpContext;
            var result = await authorizationService.AuthorizeAsync(httpContext.User, "isAdmin");
            return result.Succeeded;
        }
        public async Task LinkGenerate(AuthorDTO authorDTO)
        {
            var Url = BuildUrlHelper();
            var isAdmin = await IsAdmin();
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
    }
}
