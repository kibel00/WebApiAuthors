using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiAuthors.Services;

namespace WebApiAuthors.DTOs
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly HashService hashService;
        private readonly IDataProtector dataProtector;

        public AccountController(UserManager<IdentityUser> userManager, IConfiguration configuration, SignInManager<IdentityUser> signInManager, IDataProtectionProvider dataProtectionBuilder, HashService hashService)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
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

        [HttpPost("register")]
        public async Task<ActionResult<AnswerAuthentications>> Register(UserCredentials userCredential)
        {
            var user = new IdentityUser { UserName = userCredential.Email, Email = userCredential.Email };
            var result = await userManager.CreateAsync(user, userCredential.Password);
            if (result.Succeeded)
            {
                return await BuildToken(userCredential);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
        [HttpPost("login")]
        public async Task<ActionResult<AnswerAuthentications>> Login(UserCredentials userCredentials)
        {
            var result = await signInManager.PasswordSignInAsync(userCredentials.Email, userCredentials.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return await BuildToken(userCredentials);
            }
            else
            {
                return BadRequest("Login incorrecto");
            }
        }

        [HttpGet("tokenRenew")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<AnswerAuthentications>> ReNew()
        {
            var emailClaim = HttpContext.User.Claims.Where(claims => claims.Type == "emai").FirstOrDefault();
            var email = emailClaim.Value;
            var userCredentials = new UserCredentials() { Email = email };

            return await BuildToken(userCredentials);
        }


        private async Task<AnswerAuthentications> BuildToken(UserCredentials userCredentials)
        {
            var claims = new List<Claim>()
            {
                new Claim("email",userCredentials.Email)
            };

            var user = await userManager.FindByEmailAsync(userCredentials.Email);
            var claimDb = await userManager.GetClaimsAsync(user);

            claims.AddRange(claimDb);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTkey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMinutes(20);
            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiration, signingCredentials: creds);

            return new AnswerAuthentications()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiration = expiration
            };
        }

        [HttpPost("MakeAdmin")]
        public async Task<ActionResult> MakeAdmin(EditAdminDTO editAdminDTO)
        {
            var user = await userManager.FindByEmailAsync(editAdminDTO.Email);
            await userManager.AddClaimAsync(user, new Claim("IsAdmin", "1"));

            return NoContent();
        }


        [HttpPost("RemoveAdmin")]
        public async Task<ActionResult> RemoveAdmin(EditAdminDTO editAdminDTO)
        {
            var user = await userManager.FindByEmailAsync(editAdminDTO.Email);
            await userManager.RemoveClaimAsync(user, new Claim("IsAdmin", "1"));

            return NoContent();
        }
    }
}
