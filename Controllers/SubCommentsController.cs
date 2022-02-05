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

        public SubCommentsController(InforumBackendContext context)
        {
            _context = context;
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
                    Subcomments = paginatedSubComments,
                    Pagination = paginationMetadata
                });
            }
            catch (System.Exception)
            {

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
                    return NotFound(new
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Sub Comment not found."
                    });
                }

                return Ok(subComment);
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        // PUT: api/SubComments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubComment(long id, SubComment subComment)
        {
            if (id != subComment.Id)
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "Failed to update Sub Comment. Check if the Data is Correct."
                });
            }

            try
            {
                _context.Entry(subComment).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Sub Comment updated Successfully."
                });
            }
            catch (System.Exception)
            {
                if (!SubCommentExists(id))
                {
                    return NotFound(new
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Sub Comment not found."

                    });
                }
                else
                {
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

                return StatusCode(StatusCodes.Status201Created, new
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Sub Comment updated Succesfully."
                });
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        // DELETE: api/SubComments/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubComment(long id)
        {
            var subComment = await _context.SubComment.FindAsync(id);
            if (subComment == null)
            {
                return NotFound(new
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Sub Comment not found."

                });
            }

            try
            {
                _context.SubComment.Remove(subComment);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Sub Comment deleted Successfully."
                });
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        private bool SubCommentExists(long id)
        {
            return _context.SubComment.Any(c => c.Id == id);
        }
    }
}
