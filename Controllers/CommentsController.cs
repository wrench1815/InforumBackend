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

        public CommentsController(InforumBackendContext context)
        {
            _context = context;
        }

        // GET: api/Comments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComments([FromQuery] PageParameter pageParameter)
        {
            try
            {
                var comments = _context.Comment.Include(co => co.User).OrderByDescending(bp => bp.DatePosted);

                var paginationMetadata = new PaginationMetadata(comments.Count(), pageParameter.PageNumber, pageParameter.PageSize);
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

                var posts = await comments.Skip((pageParameter.PageNumber - 1) * pageParameter.PageSize).Take(pageParameter.PageSize).ToListAsync();

                return Ok(new
                {
                    comments = comments,
                    pagination = paginationMetadata
                });
            }
            catch (System.Exception)
            {

                return BadRequest();
            }
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(long id)
        {
            try
            {
                var comment = await _context.Comment.Include(co => co.User).FirstOrDefaultAsync(i => i.Id == id);

                if (comment == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Comment not found"
                    });
                }

                return Ok(comment);
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        // PUT: api/Comments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment(long id, Comment comment)
        {
            if (id != comment.Id)
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "Failed to update comment. Check if the Data is Correct."
                });
            }

            _context.Entry(comment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Comment not found"

                    });
                }
                else
                {
                    return BadRequest();
                }
            }

            return StatusCode(201);
        }

        // POST: api/Comments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [Authorize]
        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment(Comment comment)
        {
            try
            {
                // add a new Comment object
                _context.Comment.Add(comment);
                await _context.SaveChangesAsync(); // save the object

                // update and save the object
                // _context.Update(comment);

                // await _context.SaveChangesAsync();

                return StatusCode(201);
            }
            catch (System.Exception)
            {

                return BadRequest();
            }
        }

        // DELETE: api/Comments/5
        // [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(long id)
        {
            var comment = await _context.Comment.FindAsync(id);
            if (comment == null)
            {
                return NotFound(new
                {
                    Status = "Error",
                    Message = "Comment not found"

                });
            }

            try
            {
                _context.Comment.Remove(comment);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Status = "Success",
                    Message = "Comment deleted successfully"
                });
            }
            catch (System.Exception)
            {

                return BadRequest();
            }
        }

        private bool CommentExists(long id)
        {
            return _context.Comment.Any(c => c.Id == id);
        }
    }
}
