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
    public class EditModel : BookCategoriesPageModel
    {
        private readonly Ardelean_Victor_Dan_Lab2Context _context;

        public EditModel(Ardelean_Victor_Dan_Lab2Context context)
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
                .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (Book == null)
            {
                return NotFound();
            }

            
            PopulateAssignedCategoryData(_context, Book);

           
            ViewData["PublisherID"] = new SelectList(_context.Publisher, "ID", "PublisherName");
            var authorList = _context.Author
                 .Select(a => new
             {
                  a.ID,
                  FullName = a.LastName + " " + a.FirstName
             });
            ViewData["AuthorID"] = new SelectList(authorList, "ID", "FullName");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id, string[] selectedCategories)
        {
            if (id == null)
            {
                return NotFound();
            }

            
            var bookToUpdate = await _context.Book
                .Include(i => i.Publisher)
                .Include(i => i.Author)
                .Include(i => i.BookCategories)
                    .ThenInclude(i => i.Category)
                .FirstOrDefaultAsync(s => s.ID == id);

            if (bookToUpdate == null)
            {
                return NotFound();
            }


            if (await TryUpdateModelAsync<Book>(
                bookToUpdate,
                "Book",
                 i => i.Title, i => i.AuthorID,
                 i => i.Price, i => i.PublishingDate, i => i.PublisherID))
            {
                UpdateBookCategories(_context, selectedCategories, bookToUpdate);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }


            PopulateAssignedCategoryData(_context, bookToUpdate);
            return Page();
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.ID == id);
        }
    }
}
