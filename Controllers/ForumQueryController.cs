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

        private readonly ILogger _logger;

        public ForumQueryController(InforumBackendContext context, ILoggerFactory logger)
        {
            _context = context;
            _logger = logger.CreateLogger("ForumQueryController");
        }

        // GET: api/ForumQuery
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ForumQuery>>> GetForumQuery([FromQuery] PageParameter pageParameter, string categorySlug, string authorId, string search, Boolean voteSort)
        {
            try
            {
                IOrderedQueryable<ForumQuery> forumQuery;

                if (!String.IsNullOrEmpty(categorySlug))
                {
                    forumQuery = _context.ForumQuery.Where(fq => fq.Category.Slug == categorySlug).Include(fq => fq.Category).OrderByDescending(fq => fq.DatePosted);
                }
                else if (!String.IsNullOrEmpty(authorId))
                {
                    forumQuery = _context.ForumQuery.Where(fq => fq.AuthorId == authorId).Include(fq => fq.Category).OrderByDescending(fq => fq.DatePosted);
                }
                else if (!String.IsNullOrEmpty(search))
                {
                    forumQuery = _context.ForumQuery.Where(fq => fq.Title.Contains(search)).Include(fq => fq.Category).OrderByDescending(fq => fq.DatePosted);
                }
                else if (voteSort)
                {
                    forumQuery = _context.ForumQuery.Include(fq => fq.Category).OrderByDescending(fq => fq.Vote);
                }
                else
                {
                    forumQuery = _context.ForumQuery.Include(fq => fq.Category).OrderByDescending(fq => fq.DatePosted);
                }

                var paginationMetadata = new PaginationMetadata(forumQuery.Count(), pageParameter.PageNumber, pageParameter.PageSize);
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

                var paginatedForumQuery = await forumQuery.Skip((pageParameter.PageNumber - 1) * pageParameter.PageSize).Take(pageParameter.PageSize).ToListAsync();

                return Ok(new
                {
                    forumQuery = paginatedForumQuery,
                    pagination = paginationMetadata
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest();
            }
        }

        // GET: api/ForumQuery/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ForumQuery>> GetForumQuery(long id)
        {
            try
            {
                var forumQuery = await _context.ForumQuery.Include(fq => fq.Category).FirstOrDefaultAsync(i => i.Id == id);

                if (forumQuery == null)
                {
                    _logger.LogError("ForumQuery with id {0} not found", id);
                    return NotFound(new
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Query not found."
                    });
                }

                return Ok(forumQuery);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest();
            }
        }

        // GET: api/ForumQuery/slug/5
        [HttpGet("slug/{slug}")]
        public async Task<ActionResult<ForumQuery>> GetForumQueryBySlug(string slug)
        {
            try
            {
                var forumQuery = await _context.ForumQuery.Include(fq => fq.Category).FirstOrDefaultAsync(i => i.Slug == slug);

                if (forumQuery == null)
                {
                    _logger.LogError("ForumQuery with slug {0} not found", slug);
                    return NotFound(new
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Query not found"
                    });
                }

                return Ok(forumQuery);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest();
            }
        }

        // PUT: api/ForumQuery/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutForumQuery(long id, ForumQuery forumQuery)
        {
            try
            {
                if (id != forumQuery.Id)
                {
                    _logger.LogError("ForumQuery with id {0} not found", id);
                    return BadRequest(new
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Check if the Query Data is valid or not."
                    });
                }

                // generate slug based on the PUT data
                forumQuery.Slug = generateSlug(forumQuery.Title, id);

                _context.Entry(forumQuery).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                _logger.LogInformation("ForumQuery with id {0} updated", id);

                return StatusCode(StatusCodes.Status200OK, new
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Query updated Successfully."
                });
            }
            catch (System.Exception ex)
            {
                if (!ForumQueryExists(id))
                {
                    _logger.LogError("ForumQuery with id {0} not found", id);
                    return NotFound(new
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Query not found."
                    });
                }
                else
                {
                    _logger.LogError(ex.ToString());
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

                _logger.LogInformation("ForumQuery created Successfully");

                // Get Title and Id from the saved ForumQuery object and generate slug
                var slug = generateSlug(forumQuery.Title, forumQuery.Id);

                // assign generated slug to the ForumQuery object
                forumQuery.Slug = slug;

                _logger.LogInformation("Assign Slug: {0} to the ForumQuery object", slug);

                // update and save the object
                _context.Update(forumQuery);

                await _context.SaveChangesAsync();
                _logger.LogInformation("ForumQuery with id {0} updated", forumQuery.Id);

                return StatusCode(StatusCodes.Status201Created, new
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Query Aded Successfully."
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest();
            }
        }

        // DELETE: api/ForumQuery/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForumQuery(long id)
        {
            try
            {
                var forumQuery = await _context.ForumQuery.FindAsync(id);

                if (forumQuery == null)
                {
                    _logger.LogError("ForumQuery with id {0} not found", id);
                    return NotFound(new
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Query not found."
                    });
                }

                // Find all ForumAmswers associated with the ForumQuery
                var forumAnswers = await _context.ForumAnswer.Where(fa => fa.QueryId == id).ToListAsync();

                // Find all ForumSubAnswers of all ForumAnswers
                var forumSubAnswers = await _context.ForumSubAnswer.Where(fsa => forumAnswers.Select(fa => fa.Id).Contains(fsa.QueryAnswerId)).ToListAsync();

                // Find all Votes of ForumQuery
                var forumVotes = await _context.Vote.Where(fv => fv.ForumId == id).ToListAsync();

                // Delete all ForumSubAnswers
                _context.ForumSubAnswer.RemoveRange(forumSubAnswers);

                _logger.LogInformation("ForumSubAnswers deleted Successfully");

                // Delete all ForumAnswers
                _context.ForumAnswer.RemoveRange(forumAnswers);

                _logger.LogInformation("ForumAnswers deleted Successfully");

                // Delete all ForumVotes
                _context.Vote.RemoveRange(forumVotes);

                _logger.LogInformation("ForumVotes deleted Successfully");

                // Delete Forum Query
                _context.ForumQuery.Remove(forumQuery);
                _logger.LogInformation("ForumQuery of Id: {0} deleted Successfully", id);

                await _context.SaveChangesAsync();
                _logger.LogInformation("Save changes to the database");

                return Ok(new
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Query deleted Successfully."
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest();
            }
        }

        // POST: api/ForumQuery/vote
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost("vote")]
        public async Task<ActionResult<ForumQuery>> VoteForumQuery(Vote voteModel)
        {
            try
            {
                // Find if the forum Query Exist or not
                var forumQuery = await _context.ForumQuery.FindAsync(voteModel.ForumId);

                // Return 404 if the forum Query does not exist
                // Else Continue
                if (forumQuery == null)
                {
                    _logger.LogError("ForumQuery with id {0} not found", voteModel.ForumId);
                    return NotFound(new
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Query not found."
                    });
                }

                // Check if the Vote Entry Exist or not
                var voteExist = _context.Vote.Any(v => v.ForumId == voteModel.ForumId && v.UserId == voteModel.UserId);

                // If Vote Entry Exist remove the Vote Entry
                if (voteExist)
                {
                    // Find and remove the Vote Entry
                    var removeVote = _context.Vote.Remove(_context.Vote.FirstOrDefault(v => v.ForumId == voteModel.ForumId && v.UserId == voteModel.UserId));

                    // -1 the vote count if Vote Entry Successfully Removed
                    if (removeVote != null)
                    {
                        _logger.LogInformation("Vote Entry of ForumQuery with id {0} removed Successfully", voteModel.ForumId);
                        forumQuery.Vote--;

                        _logger.LogInformation("Vote Count of ForumQuery with id {0} Decremented Successfully", voteModel.ForumId);

                        // Save Changes to the Database
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Save changes to the database");
                    }

                    // Retutrn OK
                    return Ok(new
                    {
                        Status = StatusCodes.Status200OK,
                        Message = "Vote removed Successfully."
                    });
                }
                // Else Add the Vote Entry
                else
                {
                    // Add the Vote Entry
                    var addVote = _context.Vote.Add(voteModel);

                    // +1 the count if Vote Entry Successfully Added
                    if (addVote != null)
                    {
                        _logger.LogInformation("Vote Entry of ForumQuery with id {0} added Successfully", voteModel.ForumId);
                        forumQuery.Vote++;

                        _logger.LogInformation("Vote Count of ForumQuery with id {0} incremented Successfully", voteModel.ForumId);

                        // Save Changes to the Database
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Save changes to the database");
                    }

                    // Retutrn OK
                    return Ok(new
                    {
                        Status = StatusCodes.Status200OK,
                        Message = "Vote added Successfully."
                    });
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest();
            }
        }

        // POST: api/ForumQuery/vote/status
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost("vote/status")]
        public async Task<ActionResult<ForumQuery>> VoteStatusForumQuery(Vote voteModel)
        {
            try
            {
                // Find if the forum Query Exist or not
                var forumQuery = await _context.ForumQuery.FindAsync(voteModel.ForumId);

                // Return 404 if the forum Query does not exist
                // Else Continue
                if (forumQuery == null)
                {
                    _logger.LogError("ForumQuery with id {0} not found", voteModel.ForumId);
                    return NotFound(new
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Query not found."
                    });
                }

                // Check if the Vote Entry Exist or not
                var voteExist = _context.Vote.Any(v => v.ForumId == voteModel.ForumId && v.UserId == voteModel.UserId);

                // Retutrn OK
                return Ok(new
                {
                    VoteExist = voteExist
                });

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.ToString());
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

            _logger.LogInformation("Slug generated Successfully");

            return slug;
        }
    }
}
