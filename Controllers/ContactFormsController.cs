﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InforumBackend.Data;
using InforumBackend.Models;

namespace InforumBackend.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	public class ContactFormsController : ControllerBase {
		private readonly InforumBackendContext _context;

		public ContactFormsController(InforumBackendContext context) {
			_context = context;
		}

		// GET: api/ContactForms
		[HttpGet]
		public async Task<ActionResult<IEnumerable<ContactForm>>> GetContactForm() {
			return await _context.ContactForm.ToListAsync();
		}

		// GET: api/ContactForms/5
		[HttpGet("{id}")]
		public async Task<ActionResult<ContactForm>> GetContactForm(long id) {
			var contactForm = await _context.ContactForm.FindAsync(id);

			if (contactForm == null) {
				return NotFound();
			}

			return contactForm;
		}


		// POST: api/ContactForms
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<ActionResult<ContactForm>> PostContactForm(ContactForm contactForm) {
			_context.ContactForm.Add(contactForm);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetContactForm", new { id = contactForm.Id }, contactForm);
		}

		private bool ContactFormExists(long id) {
			return _context.ContactForm.Any(e => e.Id == id);
		}
	}
}
