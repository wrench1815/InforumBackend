using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InforumBackend.Data;
using InforumBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace InforumBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly InforumBackendContext _context;

        private readonly ILogger _logger;

        public CommentsController(InforumBackendContext context, ILoggerFactory logger)
        {
            _context = context;
            _logger = logger.CreateLogger("CommentsController");
        }

        // GET: api/Comments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComments([FromQuery] PageParameter pageParameter, long postId)
        {
            try
            {
                IOrderedQueryable<Comment> comments;

                // Get Comments by PostId and sort by Date Posted
                if (postId != 0)
                {
                    comments = _context.Comment.Where(co => co.PostId == postId).OrderByDescending(co => co.DatePosted);
                }
                // Get Comments and sort by Date Posted
                else
                {
                    comments = _context.Comment.OrderByDescending(co => co.DatePosted);
                }

                var paginationMetadata = new PaginationMetadata(comments.Count(), pageParameter.PageNumber, pageParameter.PageSize);
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

                var paginatedComments = await comments.Skip((pageParameter.PageNumber - 1) * pageParameter.PageSize).Take(pageParameter.PageSize).ToListAsync();

                return Ok(new
                {
                    Comments = paginatedComments,
                    Pagination = paginationMetadata
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest();
            }
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(long id)
        {
            try
            {
                var comment = await _context.Comment.FirstOrDefaultAsync(i => i.Id == id);

                if (comment == null)
                {
                    _logger.LogError("Comment of id: {0} not found", id);
                    return NotFound(new
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Comment not found"
                    });
                }

                return Ok(comment);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest();
            }
        }

        // PUT: api/Comments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment(long id, Comment comment)
        {
            try
            {
                if (id != comment.Id)
                {
                    _logger.LogError("Comment of id: {0} not found", id);
                    return BadRequest(new
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Failed to update comment. Check if the Data is Correct."
                    });
                }

                _context.Entry(comment).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Comment Updated Successfully."
                });
            }
            catch (System.Exception ex)
            {
                if (!CommentExists(id))
                {
                    _logger.LogError("Comment of id: {0} not found", id);
                    return NotFound(new
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Comment not found"

                    });
                }
                else
                {
                    _logger.LogError(ex.ToString());
                    return BadRequest();
                }
            }
        }

        // POST: api/Comments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment(Comment comment)
        {
            try
            {
                // add a new Comment object
                _context.Comment.Add(comment);
                await _context.SaveChangesAsync(); // save the object

                _logger.LogInformation("Comment created Successfully.");
                return StatusCode(StatusCodes.Status201Created, new
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Comment Added Succesfully."
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest();
            }
        }

        // DELETE: api/Comments/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(long id)
        {
            try
            {
                var comment = await _context.Comment.FindAsync(id);
                if (comment == null)
                {
                    _logger.LogError("Comment of id: {0} not found", id);
                    return NotFound(new
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Comment not found."

                    });
                }

                // find the subcomments of the this comment
                var subcomments = _context.SubComment.Where(sc => sc.CommentId == id);

                if (subcomments != null)
                {
                    // remove the subcomments
                    _context.SubComment.RemoveRange(subcomments);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Subcomments of Comment with id: {0} deleted Successfully.", id);
                }

                _context.Comment.Remove(comment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Comment deleted Successfully.");

                return Ok(new
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Comment deleted successfully."
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest();
            }
        }

        private bool CommentExists(long id)
        {
            return _context.Comment.Any(c => c.Id == id);
        }
    }
}
