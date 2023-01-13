using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiAuthors.DTOs;

namespace WebApiAuthors.Controllers.V1
{
    [Route("api/v1")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }


        [HttpGet("GetRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<HateOASData>>> Get()
        {
            var hateOasData = new List<HateOASData>();


            var isAdmin = await authorizationService.AuthorizeAsync(User, "isAdmin");



            hateOasData.Add(new HateOASData(link: Url.Link("GetRoot", new { }), description: "self", method: "GET"));

            hateOasData.Add(new HateOASData(link: Url.Link("getAuthors", new { }), description: "authors", method: "GET"));

            if (isAdmin.Succeeded)
            {
                hateOasData.Add(new HateOASData(link: Url.Link("authorCreate", new { }), description: "create-authors", method: "POST"));
                hateOasData.Add(new HateOASData(link: Url.Link("createBook", new { }), description: "create-book", method: "POST"));
            }


            return hateOasData;
        }
    }
}
