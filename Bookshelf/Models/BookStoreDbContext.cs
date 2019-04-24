using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Bookshelf.Models
{
    public class BookStoreDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
    }
}