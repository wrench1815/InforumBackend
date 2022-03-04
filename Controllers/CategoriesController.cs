using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InforumBackend.Data;
using InforumBackend.Models;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;
using System.Globalization;

namespace InforumBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly InforumBackendContext _context;

        private readonly ILogger _logger;

        public CategoriesController(InforumBackendContext context, ILoggerFactory logger)
        {
            _context = context;
            _logger = logger.CreateLogger("CategoriesController");
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategory()
        {
            try
            {
                var categories = await _context.Category.ToListAsync();

                return Ok(categories);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest();
            }

        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(long id)
        {
            try
            {
                var category = await _context.Category.FindAsync(id);

                if (category == null)
                {
                    _logger.LogInformation("Category of id: {0} not found.", id);
                    return NotFound(new
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Category not Found."
                    }
                    );
                }

                return Ok(category);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest();
            }
        }

        // GET: api/Categories/slug/5
        [HttpGet("slug/{slug}")]
        public async Task<ActionResult<Category>> GetCategoryBySlug(string slug)
        {
            try
            {
                var category = await _context.Category.FirstOrDefaultAsync(i => i.Name == slug);

                if (category == null)
                {
                    _logger.LogInformation("Category of slug: {0} not found.", slug);
                    return NotFound(new
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Post not found"
                    });
                }

                return Ok(category);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest();
            }
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(long id, Category category)
        {
            try
            {
                if (id != category.Id)
                {
                    _logger.LogInformation("Category of id: {0} not found.", id);
                    return BadRequest(new
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Check if request data is Correct."
                    });
                }

                // check if category with name exists
                var categoryName = await _context.Category.FirstOrDefaultAsync(i => i.Name == toTitleCase(category.Name));

                if (categoryName != null)
                {
                    _logger.LogInformation("Category with name: {0} already exists.", category.Name);
                    return BadRequest(new
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Category already exists."
                    });
                }

                // get slug for category and Titlecase Name
                var cat_slug = generateSlug(category.Name);
                category.Slug = cat_slug;
                category.Name = toTitleCase(category.Name);

                _context.Entry(category).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Category of id: {0} updated.", id);

                return Ok(new
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Category Modified Successfully."
                });
            }
            catch (System.Exception ex)
            {
                if (!CategoryExists(id))
                {
                    _logger.LogInformation("Category of id: {0} not found.", id);
                    return NotFound(new
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Category Does not Exist"
                    });
                }
                else
                {
                    _logger.LogError(ex.ToString());
                    return BadRequest();
                }
            }
        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            try
            {
                var cat_name = toTitleCase(category.Name);
                var cat_exist = await _context.Category.AnyAsync(i => i.Name == cat_name);

                if (cat_exist)
                {
                    _logger.LogInformation("Category of name: {0} already exist.", cat_name);
                    return BadRequest(new
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Category Already Exist."
                    });
                }

                var cat_slug = generateSlug(category.Name);

                category.Slug = cat_slug;
                category.Name = toTitleCase(category.Name);

                _context.Category.Add(category);

                await _context.SaveChangesAsync();
                _logger.LogInformation("Category of name: {0} created.", cat_name);

                return StatusCode(StatusCodes.Status201Created, new
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Category created Successfully.",
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest();
            }
        }

        // DELETE: api/Categories/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(long id)
        {
            try
            {
                var category = await _context.Category.FindAsync(id);

                var delCatname = toTitleCase(category.Name);

                if (category == null)
                {
                    _logger.LogInformation("Category of id: {0} not found.", id);
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Category does not exist."
                    });
                }

                if (category.Name == "General")
                {
                    _logger.LogInformation("Cannot delete General Category.");
                    return BadRequest(new
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Cannot delete General Category."
                    });
                }

                // Search category with name General
                var cat_general = await _context.Category.FirstOrDefaultAsync(i => i.Name == "General");

                // find all BlogPosts with this category and change their category with default category
                var blogPosts = await _context.BlogPost.Where(i => i.CategoryId == id).ToListAsync();

                if (blogPosts.Count > 0)
                {
                    foreach (var blogPost in blogPosts)
                    {
                        blogPost.CategoryId = cat_general.Id;

                        _logger.LogInformation("Category of BlogPost id: {0} changed to General.", blogPost.Id);
                    }

                    _context.BlogPost.UpdateRange(blogPosts);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Saved changes to BlogPosts.");
                }

                // find all ForumQueries with this category and change their category with default category
                var forumQueries = await _context.ForumQuery.Where(i => i.CategoryId == id).ToListAsync();

                if (forumQueries.Count > 0)
                {
                    foreach (var forumQuery in forumQueries)
                    {
                        forumQuery.CategoryId = cat_general.Id;

                        _logger.LogInformation("Category of ForumQuery id: {0} changed to General.", forumQuery.Id);
                    }

                    _context.ForumQuery.UpdateRange(forumQueries);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Saved changes to ForumQueries.");
                }

                _context.Category.Remove(category);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Category of id: {0} deleted.", id);

                return Ok(new
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Category Deleted Successfully."
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest();
            }
        }

        private bool CategoryExists(long id)
        {
            return _context.Category.Any(e => e.Id == id);
        }

        // Generate Slugs
        private string generateSlug(string title)
        {
            var slug = title.ToLower();

            // remove all uneeded characters
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            // remove multiple spaces
            slug = Regex.Replace(slug, @"\s+", " ").Trim();
            // replace spaces with dashes(-)
            slug = Regex.Replace(slug, @"\s", "-");

            _logger.LogInformation("Slug generated: {0}", slug);

            return slug;
        }

        // to titlecase
        private string toTitleCase(string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }
    }
}
