using Blog.Data;
using Blog.DTOs;
using Blog.Extensions;
using Blog.Models;
using Blog.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Data.Common;

namespace Blog.Controllers
{
    [Route("v1")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        [HttpGet("categories")]
        public async Task<IActionResult> Get([FromServices] BlogDataContext context, [FromServices] IMemoryCache cache)
        {
            try
            {
                var categories = cache.GetOrCreate("CategoriesCache", entry =>
                {

                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);

                    return GetCategories(context);

                });

                return Ok(new Notification<List<Category>>(categories));
            }
            catch (DbException)
            {

                return StatusCode(500, new Notification<List<Category>>("05XE10 - Não foi possível adicionar a caegoria"));
            }
            catch (Exception)
            {

                return StatusCode(500, new Notification<Category>("05XE20 - Erro interno do servidor"));
            }


        }

        [HttpGet("categories/{id:int}")]
        public async Task<IActionResult> Get([FromRoute] int id, [FromServices] BlogDataContext context)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(c => c.Id.Equals(id));
                if (category is null)
                    return NotFound(new Notification<Category>("Categoria não localizada"));

                return Ok(new Notification<Category>(category));
            }
            catch
            {
                return StatusCode(500, new Notification<Category>("05XE21 - Erro interno do servidor"));
            }
        }

        private List<Category> GetCategories(BlogDataContext context)
        {
            return context.Categories.ToList();
        }

        [HttpPost("categories")]
        public async Task<IActionResult> Post([FromBody] CreateCategoryRequest request, [FromServices] BlogDataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Notification<Category>(ModelState.GetErrors()));


            try
            {
                var category = new Category
                {
                    Id = 0,
                    Name = request.Name,
                    Slug = request.Slug.ToLower(),
                    Posts = Enumerable.Empty<Post>() as IList<Post>
                };

                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();

                return Created($"categories/{category.Id}", new Notification<Category>(category));
            }
            catch (DbUpdateException)
            {

                return StatusCode(500, new Notification<Category>("05XE12 - Não foi possíve adicionar a caegoria"));
            }
            catch
            {

                return StatusCode(500, new Notification<Category>("05XE22 - Erro interno do servidor"));
            }
        }

        [HttpPut("categories/{id:int}")]
        public async Task<IActionResult> Put([FromRoute] int id,
                                             [FromBody] UpdateCategoryRequest request,
                                             [FromServices] BlogDataContext context)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(c => c.Id.Equals(id));
                if (category is null)
                    return NotFound(new Notification<Category>("Categoria não localizada"));

                category.Name = request.Name;
                category.Slug = request.Slug;

                context.Categories.Update(category);
                await context.SaveChangesAsync();

                return Ok(new Notification<Category>(category));

            }
            catch (DbUpdateException)
            {

                return StatusCode(500, new Notification<Category>("05XE13 - Não foi possíve atualizar a caegoria"));
            }
            catch
            {

                return StatusCode(500, new Notification<Category>("05XE23 - Erro interno do servidor"));
            }


        }

        [HttpDelete("categories/{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id,
                                             [FromServices] BlogDataContext context)
        {

            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(c => c.Id.Equals(id));
                if (category is null)
                    return NotFound(new Notification<Category>("Categoria não localizada"));


                context.Categories.Remove(category);
                await context.SaveChangesAsync();

                return Ok(new Notification<Category>(category));
            }
            catch (DbException)
            {

                return StatusCode(500, new Notification<Category>("05XE14 - Não foi possíve remover a caegoria"));
            }
            catch
            {

                return StatusCode(500, new Notification<Category>("05XE24 - Erro interno do servidor"));
            }


        }
    }
}
