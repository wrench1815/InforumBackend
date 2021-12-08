﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InforumBackend.Data;
using InforumBackend.Models;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using InforumBackend.Authentication;

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
            try
            {
                return await _context.BlogPost.Include(bp => bp.Category).ToListAsync();
            }
            catch (System.Exception)
            {

                return BadRequest();
            }
        }

        // GET: api/BlogPosts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BlogPost>> GetBlogPost(long id)
        {
            try
            {
                var blogPost = await _context.BlogPost.Include(bp => bp.Category).FirstOrDefaultAsync(i => i.Id == id);

                if (blogPost == null)
                {
                    return NotFound();
                }

                return blogPost;
            }
            catch (System.Exception)
            {

                return BadRequest();
            }
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

            // generate slug based on the PUT data
            blogPost.Slug = generateSlug(blogPost.Title, id);

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
                    return BadRequest();
                }
            }

            return StatusCode(201);
        }

        // POST: api/BlogPosts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<BlogPost>> PostBlogPost(BlogPost blogPost)
        {
            try
            {
                // add a new BlogPost object
                _context.BlogPost.Add(blogPost);
                await _context.SaveChangesAsync(); // save the object

                // Get Title and Id from the saved BlogPost object and generate slug
                var slug = generateSlug(blogPost.Title, blogPost.Id);

                // assign generated slug to the BlogPost object
                blogPost.Slug = slug;

                // update and save the object
                _context.Update(blogPost);

                await _context.SaveChangesAsync();

                return StatusCode(201);
            }
            catch (System.Exception)
            {

                return BadRequest();
            }
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

            try
            {
                _context.BlogPost.Remove(blogPost);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (System.Exception)
            {

                return BadRequest();
            }
        }

        private bool BlogPostExists(long id)
        {
            return _context.BlogPost.Any(e => e.Id == id);
        }

        /// <summary>
        /// Method to generate a slug from title and id from the BlogPost object
        /// removes all the special characters and spaces and replaces them with dashes(-)
        /// concatenates cleaned title and id and returns the slug in format of title-id
        /// </summary>
        /// <param name="title"></param>
        /// <param name="id"></param>
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
