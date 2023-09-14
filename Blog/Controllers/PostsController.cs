using Blog.Data;
using Blog.DTOs;
using Blog.Models;
using Blog.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    // [Authorize]
    [Route("v1")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        [HttpGet("posts")]
        public async Task<IActionResult> Get([FromServices] BlogDataContext context)
        {
            try
            {
                var posts = await context.Posts
               .AsNoTracking()
               .Include(c => c.Category)
               .Include(c => c.Author)
               .OrderByDescending(c => c.LastUpdateDate)
               .Select(x => new ListPostRequest
               {
                   Id = x.Id,
                   Title = x.Title,
                   Slug = x.Slug,
                   Category = x.Category.Name,
                   Author = $"{x.Author.Name} ({x.Author.Email})"

               })
               .ToListAsync();

                return Ok(new Notification<dynamic>(new
                {
                    total = posts.Count,
                    posts

                }));
            }
            catch (Exception)
            {

                return StatusCode(500, new Notification<Category>("05XE21 - Erro interno do servidor"));
            }
        }


        [HttpGet("posts/category/{category}")]
        public async Task<IActionResult> GetOnCategory([FromRoute] string category, [FromServices] BlogDataContext context)
        {
            try
            {
                var count = await context.Posts
               .AsNoTracking()
               .CountAsync();

                var posts = await context.Posts
                    .AsNoTracking()
                    .Include(c => c.Author)
                    .Include(c => c.Category)
                    .Where(c => c.Category.Slug.Equals(category))
                    .Select(x => new ListPostRequest
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Slug = x.Slug,
                        LastUpdateDate = x.LastUpdateDate,
                        Category = x.Category.Name,
                        Author = $"{x.Author.Name} ({x.Author.Email})"
                    })
                    .OrderByDescending(c => c.LastUpdateDate)
                    .ToListAsync();

                return Ok(new Notification<dynamic>(new
                {
                    total = count,
                    posts

                }));
            }
            catch (Exception)
            {

                return StatusCode(500, new Notification<Post>("05XE21 - Erro interno do servidor"));
            }

        }

        [HttpGet("posts/{id:int}")]
        public async Task<IActionResult> Details([FromServices] BlogDataContext context, [FromRoute] int id)
        {
            try
            {
                var posts = await context.Posts
               .AsNoTracking()
               .Include(c => c.Author)
               .ThenInclude(c => c.Roles)
               .Include(c => c.Category)
               .FirstOrDefaultAsync(c => c.Id.Equals(id));

                if (posts is null)
                    return NotFound(new Notification<Post>("Post não localizado"));

                return Ok(new Notification<Post>(posts));
            }
            catch (Exception)
            {
                return StatusCode(500, new Notification<Category>("05XE21 - Erro interno do servidor"));
            }
        }
    }
}



