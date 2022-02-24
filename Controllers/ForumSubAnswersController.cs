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
    public class ForumSubAnswersController : ControllerBase
    {
        private readonly InforumBackendContext _context;

        private readonly ILogger _logger;

        public ForumSubAnswersController(InforumBackendContext context, ILoggerFactory logger)
        {
            _context = context;
            _logger = logger.CreateLogger("ForumSubAnswersController");
        }

        // GET: api/ForumSubAnswers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ForumSubAnswer>>> GetForumSubAnswers([FromQuery] PageParameter pageParameter, long queryAnswerId)
        {
            try
            {
                IOrderedQueryable<ForumSubAnswer> subAnswers;

                if (queryAnswerId != 0)
                {
                    subAnswers = _context.ForumSubAnswer.Where(sc => sc.QueryAnswerId == queryAnswerId).OrderByDescending(sc => sc.DatePosted);
                }
                else
                {
                    subAnswers = _context.ForumSubAnswer.OrderByDescending(sc => sc.DatePosted);
                }

                var paginationMetadata = new PaginationMetadata(subAnswers.Count(), pageParameter.PageNumber, pageParameter.PageSize);
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

                var paginatedSubAnswers = await subAnswers.Skip((pageParameter.PageNumber - 1) * pageParameter.PageSize).Take(pageParameter.PageSize).ToListAsync();

                return Ok(new
                {
                    SubAnswers = paginatedSubAnswers,
                    Pagination = paginationMetadata
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest();
            }
        }

        // GET: api/ForumSubAnswers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ForumSubAnswer>> GetForumSubAnswer(long id)
        {
            try
            {
                var subAnswer = await _context.ForumSubAnswer.FirstOrDefaultAsync(i => i.Id == id);

                if (subAnswer == null)
                {
                    _logger.LogError("ForumSubAnswer with id {id} not found", id);
                    return NotFound(new
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Sub Answer not found."
                    });
                }

                return Ok(subAnswer);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest();
            }
        }

        // PUT: api/ForumSubAnswers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubAnswer(long id, ForumSubAnswer subAnswer)
        {
            try
            {
                if (id != subAnswer.Id)
                {
                    _logger.LogError("ForumSubAnswer with id {id} not found", id);
                    return BadRequest(new
                    {
                        Status = "Error",
                        Message = "Failed to update Sub Answer. Check if the Data is Correct."
                    });
                }

                _context.Entry(subAnswer).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                _logger.LogInformation("ForumSubAnswer with id {id} updated", id);

                return Ok(new
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Sub Answer updated Successfully."
                });
            }
            catch (System.Exception ex)
            {
                if (!SubAnswerExists(id))
                {
                    _logger.LogError("ForumSubAnswer with id {id} not found", id);
                    return NotFound(new
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Sub Answer not found."

                    });
                }
                else
                {
                    _logger.LogError(ex.ToString());
                    return BadRequest();
                }
            }
        }

        // POST: api/ForumSubAnswers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ForumSubAnswer>> PostForumSubAnswer(ForumSubAnswer subAnswer)
        {
            try
            {
                // add a new Sub Answer object
                _context.ForumSubAnswer.Add(subAnswer);
                await _context.SaveChangesAsync(); // save the object

                _logger.LogInformation("SubAnswer Created Successfully");

                return StatusCode(StatusCodes.Status201Created, new
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Sub Answer added Succesfully."
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest();
            }
        }

        // DELETE: api/ForumSubAnswers/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForumSubAnswer(long id)
        {
            try
            {
                var subAnswer = await _context.ForumSubAnswer.FindAsync(id);

                if (subAnswer == null)
                {
                    _logger.LogError("ForumSubAnswer with id {id} not found", id);
                    return NotFound(new
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Sub Answer not found."
                    });
                }

                _context.ForumSubAnswer.Remove(subAnswer);
                await _context.SaveChangesAsync();

                _logger.LogInformation("ForumSubAnswer with id {id} deleted", id);

                return Ok(new
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Sub Answer deleted Successfully."
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest();
            }
        }

        private bool SubAnswerExists(long id)
        {
            return _context.ForumSubAnswer.Any(c => c.Id == id);
        }
    }
}
