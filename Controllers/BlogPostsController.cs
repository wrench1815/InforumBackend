using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InforumBackend.Data;
using InforumBackend.Models;

namespace InforumBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private readonly InforumBackendContext _context;

        public BlogPostsController(InforumBackendContext context)
        {
            _context = context;
        }

        // GET: api/BlogPosts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetBlogPost()
        {

            return await _context.BlogPost.Include(bp => bp.Category).ToListAsync();
        }

        // GET: api/BlogPosts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BlogPost>> GetBlogPost(long id)
        {
            var blogPost = await _context.BlogPost.Include(bp => bp.Category).FirstOrDefaultAsync(i => i.Id == id);

            if (blogPost == null)
            {
                return NotFound();
            }

            return blogPost;
        }

        // PUT: api/BlogPosts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBlogPost(long id, BlogPost blogPost)
        {
            if (id != blogPost.Id)
            {
                return BadRequest();
            }

            _context.Entry(blogPost).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogPostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/BlogPosts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BlogPost>> PostBlogPost(BlogPost blogPost)
        {
            _context.BlogPost.Add(blogPost);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBlogPost", new { id = blogPost.Id }, blogPost);
        }

        // DELETE: api/BlogPosts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogPost(long id)
        {
            var blogPost = await _context.BlogPost.FindAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }

            _context.BlogPost.Remove(blogPost);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BlogPostExists(long id)
        {
            return _context.BlogPost.Any(e => e.Id == id);
        }
    }
}
