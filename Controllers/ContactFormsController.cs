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
        public async Task<ActionResult<IEnumerable<ContactForm>>> GetContactForm()
        {
            return await _context.ContactForm.ToListAsync();
        }

        // GET: api/ContactForms/5
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ContactForm>> GetContactForm(long id)
        {
            var contactForm = await _context.ContactForm.FindAsync(id);

            if (contactForm == null)
            {
                return NotFound();
            }

            return contactForm;
        }

        // PUT: api/ContactForms/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContactForm(long id, ContactForm contactForm)
        {
            if (id != contactForm.Id)
            {
                return BadRequest();
            }

            _context.Entry(contactForm).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactFormExists(id))
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

        // POST: api/ContactForms
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ContactForm>> PostContactForm(ContactForm contactForm)
        {
            _context.ContactForm.Add(contactForm);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContactForm", new { id = contactForm.Id }, contactForm);
        }

        // DELETE: api/ContactForms/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContactForm(long id)
        {
            var contactForm = await _context.ContactForm.FindAsync(id);
            if (contactForm == null)
            {
                return NotFound();
            }

            _context.ContactForm.Remove(contactForm);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ContactFormExists(long id)
        {
            return _context.ContactForm.Any(e => e.Id == id);
        }
    }
}
