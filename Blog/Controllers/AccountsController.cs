using Blog.Api.Dtos;
using Blog.Api.Services;
using Blog.Data;
using Blog.Dtos;
using Blog.Extensions;
using Blog.Models;
using Blog.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace Blog.Api.Controllers
{
    [Route("v1")]
    [ApiController]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly TokenService _tokenService;
        public AccountsController(TokenService tokenService)
        {
            _tokenService = tokenService;
        }
        [AllowAnonymous]
        [HttpPost("accounts/")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request,
                                              [FromServices] BlogDataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Notification<string>(ModelState.GetErrors()));

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Slug = request.Email.Replace("@", "-").Replace(".", "-")
            };

            //Gera senha com 16 caracteres
            var password = PasswordGenerator.Generate(16, includeSpecialChars: false);
            user.PasswordHash = PasswordHasher.Hash(password);

            try
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();


                return Ok(new Notification<dynamic>(new { user = user.Email, password }));

            }
            catch (DbUpdateException)
            {
                return StatusCode(400, new Notification<string>("05X99 - Email já cadastrado"));
            }
            catch
            {

                return StatusCode(500, new Notification<string>("05X04 - Erro interno no servidor"));
            }

        }

        //[Authorize(Roles = "admin")]
        //[Authorize(Roles = "user")]
        //[HttpGet("user")]
        //public IActionResult GetUser() => Ok(User.Identity.Name);

        //[Authorize(Roles = "admin")]
        //[Authorize(Roles = "author")]
        //[HttpGet("author")]
        //public IActionResult GetAuthor() => Ok(User.Identity.Name);

        //[Authorize(Roles = "admin")]
        //[HttpGet("admin")]
        //public IActionResult GetAdmin() => Ok(User.Identity.Name);



        [AllowAnonymous]
        [HttpPost("accounts/login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request,
                                               [FromServices] BlogDataContext context)
        {

            if (!ModelState.IsValid)
                return BadRequest(new Notification<string>(ModelState.GetErrors()));

            var user = await context
                .Users
                .AsNoTracking()
                .Include(r => r.Roles)
                .FirstOrDefaultAsync(x => x.Email.Equals(request.Email));

            if (user is null)
                return StatusCode(401, new Notification<string>("05X89 - Usuário ou senha iválidos"));

            if (!PasswordHasher.Verify(user.PasswordHash, request.Password))
                return StatusCode(401, new Notification<string>("05X89 - Usuário ou senha iválidos"));

            try
            {
                var token = _tokenService.GenerateToken(user);
                return Ok(new Notification<string>(token, null));
            }
            catch
            {

                return StatusCode(500, new Notification<string>("05X04 - Erro interno no servidor"));
            }

        }


    }
}
