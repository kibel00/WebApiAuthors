﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApiAuthors.DTOs
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;

        public AccountController(UserManager<IdentityUser> userManager, IConfiguration configuration, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
        }
        [HttpPost("register")]
        public async Task<ActionResult<AnswerAuthentications>> Register(UserCredentials userCredential)
        {
            var user = new IdentityUser { UserName = userCredential.Email, Email = userCredential.Email };
            var result = await userManager.CreateAsync(user, userCredential.Password);
            if (result.Succeeded)
            {
                return BuildToken(userCredential);
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
                return BuildToken(userCredentials);
            }
            else
            {
                return BadRequest("Login incorrecto");
            }
        }

        [HttpGet("tokenRenew")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult<AnswerAuthentications> ReNew()
        {
            var emailClaim = HttpContext.User.Claims.Where(claims => claims.Type == "emai").FirstOrDefault();
            var email = emailClaim.Value;
            var userCredentials = new UserCredentials() { Email = email };

            return BuildToken(userCredentials);
        }


        private AnswerAuthentications BuildToken(UserCredentials userCredentials)
        {
            var claims = new List<Claim>()
            {
                new Claim("email",userCredentials.Email)
            };
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
    }
}
