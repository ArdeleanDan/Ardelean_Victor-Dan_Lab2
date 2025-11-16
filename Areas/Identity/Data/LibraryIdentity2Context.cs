using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ardelean_Victor_Dan_Lab2.Data
{
    public class LibraryIdentity2Context : IdentityDbContext
    {
        public LibraryIdentity2Context(DbContextOptions<LibraryIdentity2Context> options)
            : base(options)
        {
        }
    }
}
