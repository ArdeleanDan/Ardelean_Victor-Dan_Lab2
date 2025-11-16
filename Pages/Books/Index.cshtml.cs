using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ardelean_Victor_Dan_Lab2.Data;
using Ardelean_Victor_Dan_Lab2.Models;
using System.Linq;
using Ardelean_Victor_Dan_Lab2.Models.View_Models;

namespace Ardelean_Victor_Dan_Lab2.Pages.Books
{
    public class IndexModel : PageModel
    {
        private readonly Ardelean_Victor_Dan_Lab2Context _context;

        public IndexModel(Ardelean_Victor_Dan_Lab2Context context)
        {
            _context = context;
        }

        public IList<Book> Book { get; set; }
        public BookData BookD { get; set; }
        public int BookID { get; set; }
        public int CategoryID { get; set; }

        public string TitleSort { get; set; }
        public string AuthorSort { get; set; }
        public string CurrentFilter { get; set; }



        public async Task OnGetAsync(int? id, int? categoryID, string sortOrder, string searchString)
        {
            TitleSort = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            AuthorSort = sortOrder == "author" ? "author_desc" : "author";
            CurrentFilter = searchString;

            
            var booksIQ = from b in _context.Book
                          .Include(b => b.Author)
                          .Include(b => b.Publisher)
                          .Include(b => b.BookCategories)
                              .ThenInclude(bc => bc.Category)
                          select b;

            
            if (!String.IsNullOrEmpty(searchString))
            {
                booksIQ = booksIQ.Where(s =>
                    s.Title.Contains(searchString) ||
                    s.Author.FirstName.Contains(searchString) ||
                    s.Author.LastName.Contains(searchString));
            }

            
            switch (sortOrder)
            {
                case "title_desc":
                    booksIQ = booksIQ.OrderByDescending(s => s.Title);
                    break;
                case "author_desc":
                    booksIQ = booksIQ.OrderByDescending(s => s.Author.LastName);
                    break;
                case "author":
                    booksIQ = booksIQ.OrderBy(s => s.Author.LastName);
                    break;
                default:
                    booksIQ = booksIQ.OrderBy(s => s.Title);
                    break;
            }

            
            BookD = new BookData
            {
                Books = await booksIQ.AsNoTracking().ToListAsync()
            };

            
            if (id != null)
            {
                BookID = id.Value;
                Book book = BookD.Books.Single(i => i.ID == id.Value);
                BookD.Categories = book.BookCategories.Select(s => s.Category);
            }
        }
    }
}
