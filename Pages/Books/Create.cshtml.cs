using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ardelean_Victor_Dan_Lab2.Data;
using Ardelean_Victor_Dan_Lab2.Models;
using Microsoft.EntityFrameworkCore;

namespace Ardelean_Victor_Dan_Lab2.Pages.Books
{
    public class CreateModel : BookCategoriesPageModel
    {
        private readonly Ardelean_Victor_Dan_Lab2.Data.Ardelean_Victor_Dan_Lab2Context _context;

        public CreateModel(Ardelean_Victor_Dan_Lab2.Data.Ardelean_Victor_Dan_Lab2Context context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            
            var authorList = _context.Author
                .Select(a => new
                {
                    a.ID,
                    FullName = a.LastName + " " + a.FirstName
                }).ToList();

            ViewData["AuthorID"] = new SelectList(authorList, "ID", "FullName");

           
            ViewData["PublisherID"] = new SelectList(_context.Publisher, "ID", "PublisherName");

            var book = new Book();
            book.BookCategories = new List<BookCategory>();
            PopulateAssignedCategoryData(_context, book);

            return Page();
        }


        [BindProperty]
        public Book Book { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(string[] selectedCategories)
        {
            
            if (!ModelState.IsValid)
            {
                var authorList = _context.Author
                    .Select(a => new
                    {
                        a.ID,
                        FullName = a.LastName + " " + a.FirstName
                    }).ToList();
                ViewData["AuthorID"] = new SelectList(authorList, "ID", "FullName");
                ViewData["PublisherID"] = new SelectList(_context.Publisher, "ID", "PublisherName");

                PopulateAssignedCategoryData(_context, Book);
                return Page();
            }

            
            var newBook = new Book();
            if (selectedCategories != null)
            {
                newBook.BookCategories = new List<BookCategory>();
                foreach (var cat in selectedCategories)
                {
                    var catToAdd = new BookCategory
                    {
                        CategoryID = int.Parse(cat)
                    };
                    newBook.BookCategories.Add(catToAdd);
                }
            }

            Book.BookCategories = newBook.BookCategories;
            _context.Book.Add(Book);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}