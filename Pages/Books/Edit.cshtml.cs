using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ardelean_Victor_Dan_Lab2.Data;
using Ardelean_Victor_Dan_Lab2.Models;

namespace Ardelean_Victor_Dan_Lab2.Pages.Books
{
    public class EditModel : PageModel
    {
        private readonly Ardelean_Victor_Dan_Lab2.Data.Ardelean_Victor_Dan_Lab2Context _context;

        public EditModel(Ardelean_Victor_Dan_Lab2.Data.Ardelean_Victor_Dan_Lab2Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Book Book { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Book = await _context.Book
                .Include(b => b.Publisher)
                .Include(b => b.Author)
                .FirstOrDefaultAsync(m => m.ID == id)
                ?? new Book();

            if (Book == null)
            {
                return NotFound();
            }

            ViewData["PublisherID"] = new SelectList(_context.Publisher, "ID", "PublisherName");
            ViewData["AuthorID"] = new SelectList(_context.Author, "ID", "LastName");
            return Page();
        }


        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(Book.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.ID == id);
        }
    }
}
