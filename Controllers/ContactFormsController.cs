using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
    public class ContactFormsController : ControllerBase
    {
        private readonly InforumBackendContext _context;

        public ContactFormsController(InforumBackendContext context)
        {
            _context = context;
        }

        // GET: api/ContactForms
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactForm>>> GetContactForm([FromQuery] PageParameter pageParameter)
        {
            var contactForms = _context.ContactForm.OrderByDescending(cf => cf.CreatedOn);

            var paginationMetadata = new PaginationMetadata(contactForms.Count(), pageParameter.PageNumber, pageParameter.PageSize);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

            var forms = await contactForms.Skip((pageParameter.PageNumber - 1) * pageParameter.PageSize).Take(pageParameter.PageSize).ToListAsync();

            return Ok(new
            {
                forms = forms,
                pagination = paginationMetadata
            });
        }

        // GET: api/ContactForms/5
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ContactForm>> GetContactForm(long id)
        {
            var contactForm = await _context.ContactForm.FindAsync(id);

            if (contactForm == null)
            {
                return NotFound(new
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Contact Form not Fount."
                });
            }

            return Ok(contactForm);
        }

        // PUT: api/ContactForms/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContactForm(long id, ContactForm contactForm)
        {
            if (id != contactForm.Id)
            {
                return BadRequest(new
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Check if Form Data is Valid or not."
                });
            }

            _context.Entry(contactForm).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK, new
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Contact Form Updated Successfully."
                });
            }
            catch (System.Exception)
            {
                if (!ContactFormExists(id))
                {
                    return NotFound(new
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Contact Form not Found."
                    });
                }
                else
                {
                    return BadRequest();
                }
            }
        }

        // POST: api/ContactForms
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ContactForm>> PostContactForm(ContactForm contactForm)
        {
            try
            {
                _context.ContactForm.Add(contactForm);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Contact Form added Successfully."
                });
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        // DELETE: api/ContactForms/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContactForm(long id)
        {
            var contactForm = await _context.ContactForm.FindAsync(id);
            if (contactForm == null)
            {
                return NotFound(new
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Contact Form not Found."
                });
            }
            try
            {
                _context.ContactForm.Remove(contactForm);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Contact Form deleted Successfully."
                });
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        private bool ContactFormExists(long id)
        {
            return _context.ContactForm.Any(e => e.Id == id);
        }
    }
}
