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
    public class ForumAnswerController : ControllerBase
    {
        private readonly InforumBackendContext _context;

        public ForumAnswerController(InforumBackendContext context)
        {
            _context = context;
        }

        // GET: api/ForumAnswer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ForumAnswer>>> GetForumAnswer([FromQuery] PageParameter pageParameter, long queryId)
        {
            try
            {
                IOrderedQueryable<ForumAnswer> forumAnswerList;

                if (queryId != 0)
                {
                    forumAnswerList = _context.ForumAnswer.Where(fa => fa.QueryId == queryId).OrderByDescending(fa => fa.DatePosted);
                }
                else
                {
                    forumAnswerList = _context.ForumAnswer.OrderByDescending(fa => fa.DatePosted);
                }

                var paginationMetadata = new PaginationMetadata(forumAnswerList.Count(), pageParameter.PageNumber, pageParameter.PageSize);
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

                var forumAnswerPaginated = await forumAnswerList.Skip((pageParameter.PageNumber - 1) * pageParameter.PageSize).Take(pageParameter.PageSize).ToListAsync();

                return Ok(new
                {
                    Answers = forumAnswerPaginated,
                    Pagination = paginationMetadata
                });
            }
            catch (System.Exception)
            {

                return BadRequest();
            }
        }

        // GET: api/ForumAnswer/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ForumAnswer>> GetForumAnswer(long id)
        {
            try
            {
                var forumAnswer = await _context.ForumAnswer.FirstOrDefaultAsync(i => i.Id == id);

                if (forumAnswer == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "Answer not found"
                    });
                }

                return Ok(forumAnswer);
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        // PUT: api/ForumAnswer/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutForumAnswer(long id, ForumAnswer forumAnswer)
        {
            if (id != forumAnswer.Id)
            {
                return BadRequest(new
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Failed to update Answer. Check if the Data is Correct."
                });
            }

            _context.Entry(forumAnswer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Answer Updates Successfully."
                });
            }
            catch (System.Exception)
            {
                if (!ForumAnswerExists(id))
                {
                    return NotFound(new
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Answer not found."
                    });
                }
                else
                {
                    return BadRequest();
                }
            }
        }

        // POST: api/ForumAnswer
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ForumAnswer>> PostForumAnswer(ForumAnswer forumAnswer)
        {
            try
            {
                // add a new ForumAnswer object
                _context.ForumAnswer.Add(forumAnswer);
                await _context.SaveChangesAsync(); // save the object

                return StatusCode(StatusCodes.Status201Created, new
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Answer added Successfully."
                });
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        // DELETE: api/ForumAnswer/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForumAnswer(long id)
        {
            var forumAnswer = await _context.ForumAnswer.FindAsync(id);
            if (forumAnswer == null)
            {
                return NotFound(new
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Answer not found."

                });
            }

            try
            {
                _context.ForumAnswer.Remove(forumAnswer);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Answer deleted successfully."
                });
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        private bool ForumAnswerExists(long id)
        {
            return _context.ForumAnswer.Any(fa => fa.Id == id);
        }
    }
}
