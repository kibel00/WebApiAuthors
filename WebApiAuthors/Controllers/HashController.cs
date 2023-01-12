using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using WebApiAuthors.Services;

namespace WebApiAuthors.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HashController : ControllerBase
    {
        private readonly HashService hashService;
        private readonly IDataProtector dataProtector;
        public HashController(IDataProtectionProvider dataProtectionBuilder, HashService hashService)
        {
            this.hashService = hashService;
            this.dataProtector = dataProtectionBuilder.CreateProtector("unique_value_and_maybe_secret");
        }
        [HttpGet("hasg/{flatText}")]
        public IActionResult MakeHash(string flatText)
        {
            var result1 = hashService.Hash(flatText);
            var result2 = hashService.Hash(flatText);
            return Ok(new
            {
                flatText = flatText,
                result1 = result1,
                result2 = result2
            });
        }

        [HttpGet("encrypt")]
        public IActionResult Encrypt()
        {
            var flatText = "Santo Herrera";
            var flatProtected = dataProtector.Protect(flatText);
            var flatUnprotect = dataProtector.Unprotect(flatProtected);

            return Ok(new
            {
                flatText = flatText,
                flatProtected = flatProtected,
                flatUnprotect = flatUnprotect
            });
        }


        [HttpGet("encryptByTime")]
        public IActionResult EncryptByTime()
        {


            var limitedProtectedByTime = dataProtector.ToTimeLimitedDataProtector();
            var flatText = "Santo Herrera";
            var flatProtected = limitedProtectedByTime.Protect(flatText, lifetime: TimeSpan.FromSeconds(5));
            var flatUnprotect = limitedProtectedByTime.Unprotect(flatProtected);

            return Ok(new
            {
                flatText = flatText,
                flatProtected = flatProtected,
                flatUnprotect = flatUnprotect
            });
        }
    }
}
