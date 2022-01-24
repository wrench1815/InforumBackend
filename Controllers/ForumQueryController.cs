using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InforumBackend.Data;
using InforumBackend.Models;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace InforumBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForumQueryController : ControllerBase
    {
        private readonly InforumBackendContext _context;

        public ForumQueryController(InforumBackendContext context)
        {
            _context = context;
        }

        // GET: api/ForumQuery
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ForumQuery>>> GetForumQuery([FromQuery] PageParameter pageParameter)
        {
            try
            {
                var forumQuery = _context.ForumQuery.Include(fq => fq.Category).Include(fq => fq.Author).OrderByDescending(fq => fq.DatePosted);

                var paginationMetadata = new PaginationMetadata(forumQuery.Count(), pageParameter.PageNumber, pageParameter.PageSize);
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

                var paginatedForumQuery = await forumQuery.Skip((pageParameter.PageNumber - 1) * pageParameter.PageSize).Take(pageParameter.PageSize).ToListAsync();

                return Ok(new
                {
                    Forumquery = paginatedForumQuery,
                    pagination = paginationMetadata
                });
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        // GET: api/ForumQuery/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ForumQuery>> GetForumQuery(long id)
        {
            try
            {
                var forumQuery = await _context.ForumQuery.Include(fq => fq.Category).Include(fq => fq.Author).FirstOrDefaultAsync(i => i.Id == id);

                if (forumQuery == null)
                {
                    return NotFound(new
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Query not found."
                    });
                }

                return Ok(forumQuery);
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        // PUT: api/ForumQuery/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutForumQuery(long id, ForumQuery forumQuery)
        {
            if (id != forumQuery.Id)
            {
                return BadRequest(new
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Check if the Query Data is valid or not."
                });
            }

            // generate slug based on the PUT data
            forumQuery.Slug = generateSlug(forumQuery.Title, id);

            _context.Entry(forumQuery).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK, new
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Query updated Successfully."
                });
            }
            catch (System.Exception)
            {
                if (!ForumQueryExists(id))
                {
                    return NotFound(new
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Query not found."
                    });
                }
                else
                {
                    return BadRequest();
                }
            }
        }

        // POST: api/ForumQuery
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ForumQuery>> PostForumQuery(ForumQuery forumQuery)
        {
            try
            {
                // add a new ForumQuery object
                _context.ForumQuery.Add(forumQuery);
                await _context.SaveChangesAsync(); // save the object

                // Get Title and Id from the saved ForumQuery object and generate slug
                var slug = generateSlug(forumQuery.Title, forumQuery.Id);

                // assign generated slug to the ForumQuery object
                forumQuery.Slug = slug;

                // update and save the object
                _context.Update(forumQuery);

                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK, new
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Query Aded Successfully."
                });
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        // DELETE: api/ForumQuery/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForumQuery(long id)
        {
            var forumQuery = await _context.ForumQuery.FindAsync(id);
            if (forumQuery == null)
            {
                return NotFound(new
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Query not found."
                });
            }

            try
            {
                _context.ForumQuery.Remove(forumQuery);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Query deleted Successfully."
                });
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        private bool ForumQueryExists(long id)
        {
            return _context.ForumQuery.Any(e => e.Id == id);
        }

        /// <summary>
        /// Method to generate a slug from title and id from an object
        /// removes all the special characters and spaces and replaces them with dashes(-)
        /// concatenates cleaned title and id and returns the slug in format of title-id
        /// </summary>
        /// <param name="title">Title of Object</param>
        /// <param name="id">Id of the Object</param>
        /// <returns>slug(title-id)</returns>
        private string generateSlug(string title, long id)
        {
            var slug = title.ToLower();

            // remove all uneeded characters
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            // remove multiple spaces
            slug = Regex.Replace(slug, @"\s+", " ").Trim();
            // replace spaces with dashes(-)
            slug = Regex.Replace(slug, @"\s", "-");
            // concatenate slug and id
            slug = slug + "-" + id;

            return slug;
        }
    }
}
