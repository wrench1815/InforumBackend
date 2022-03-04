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
    public class SubCommentsController : ControllerBase
    {
        private readonly InforumBackendContext _context;

        private readonly ILogger _logger;

        public SubCommentsController(InforumBackendContext context, ILoggerFactory logger)
        {
            _context = context;
            _logger = logger.CreateLogger("SubCommentsController");
        }

        // GET: api/SubComments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubComment>>> GetSubComments([FromQuery] PageParameter pageParameter, long commentId)
        {
            try
            {
                IOrderedQueryable<SubComment> subComments;

                if (commentId != 0)
                {
                    subComments = _context.SubComment.Where(sc => sc.CommentId == commentId).OrderByDescending(sc => sc.DatePosted);
                }
                else
                {
                    subComments = _context.SubComment.OrderByDescending(sc => sc.DatePosted);
                }

                var paginationMetadata = new PaginationMetadata(subComments.Count(), pageParameter.PageNumber, pageParameter.PageSize);
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

                var paginatedSubComments = await subComments.Skip((pageParameter.PageNumber - 1) * pageParameter.PageSize).Take(pageParameter.PageSize).ToListAsync();

                return Ok(new
                {
                    SubComments = paginatedSubComments,
                    Pagination = paginationMetadata
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest();
            }
        }

        // GET: api/SubComments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubComment>> GetSubComment(long id)
        {
            try
            {
                var subComment = await _context.SubComment.FirstOrDefaultAsync(i => i.Id == id);

                if (subComment == null)
                {
                    _logger.LogError("SubComment with id {0} not found", id);
                    return NotFound(new
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Sub Comment not found."
                    });
                }

                return Ok(subComment);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest();
            }
        }

        // PUT: api/SubComments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubComment(long id, SubComment subComment)
        {
            try
            {
                if (id != subComment.Id)
                {
                    _logger.LogError("SubComment with id {0} not found", id);
                    return BadRequest(new
                    {
                        Status = "Error",
                        Message = "Failed to update Sub Comment. Check if the Data is Correct."
                    });
                }

                _context.Entry(subComment).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                _logger.LogInformation("SubComment with id {0} updated", id);

                return Ok(new
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Sub Comment updated Successfully."
                });
            }
            catch (System.Exception ex)
            {
                if (!SubCommentExists(id))
                {
                    _logger.LogError("SubComment with id {0} not found", id);
                    return NotFound(new
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Sub Comment not found."

                    });
                }
                else
                {
                    _logger.LogError(ex.ToString());
                    return BadRequest();
                }
            }
        }

        // POST: api/SubComments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<SubComment>> PostSubComment(SubComment subComment)
        {
            try
            {
                // add a new Sub Comment object
                _context.SubComment.Add(subComment);
                await _context.SaveChangesAsync(); // save the object

                _logger.LogInformation("SubComment created Successfully");

                return StatusCode(StatusCodes.Status201Created, new
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Sub Comment added Succesfully."
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest();
            }
        }

        // DELETE: api/SubComments/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubComment(long id)
        {
            try
            {
                var subComment = await _context.SubComment.FindAsync(id);

                if (subComment == null)
                {
                    _logger.LogError("SubComment with id {0} not found", id);
                    return NotFound(new
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Sub Comment not found."

                    });
                }

                _context.SubComment.Remove(subComment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("SubComment with id {0} deleted", id);

                return Ok(new
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Sub Comment deleted Successfully."
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest();
            }
        }

        private bool SubCommentExists(long id)
        {
            return _context.SubComment.Any(c => c.Id == id);
        }
    }
}
