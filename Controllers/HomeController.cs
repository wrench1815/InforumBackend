using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InforumBackend.Data;
using InforumBackend.Models;
using Microsoft.AspNetCore.Authorization;

namespace InforumBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly InforumBackendContext _context;

        public HomeController(InforumBackendContext context)
        {
            _context = context;
        }

        // GET: api/Home
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Home>>> GetHome()
        {
            var homeData = await _context.Home.ToListAsync();

            return Ok(homeData);
        }

        // GET: api/Home/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Home>> GetHome(long id)
        {
            try
            {
                var home = await _context.Home.FindAsync(id);

                if (home == null)
                {
                    return NotFound(new
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Home data not found."
                    });
                }

                return Ok(home);
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        // PUT: api/Home/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHome(long id, Home home)
        {
            if (id != home.Id)
            {
                return BadRequest(new
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Check if Home data is Valid or not."
                });
            }

            _context.Entry(home).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Home data updated Successfully."
                });
            }
            catch (System.Exception)
            {
                if (!HomeExists(id))
                {
                    return NotFound(new
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Home does not Exist."
                    });
                }
                else
                {
                    return BadRequest();
                }
            }
        }

        // POST: api/Home
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Home>> PostHome(Home home)
        {
            try
            {
                _context.Home.Add(home);
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status201Created, new
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Home data added Successfully."
                });
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        // DELETE: api/Home/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHome(long id)
        {
            try
            {
                var home = await _context.Home.FindAsync(id);
                if (home == null)
                {
                    return NotFound();
                }

                _context.Home.Remove(home);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Home deleted Successfully."
                });
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        private bool HomeExists(long id)
        {
            return _context.Home.Any(e => e.Id == id);
        }
    }
}
