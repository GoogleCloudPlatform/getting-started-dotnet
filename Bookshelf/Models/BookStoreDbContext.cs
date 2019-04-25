using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Bookshelf.Models
{
    public class BookStoreDbContext : DbContext
    {
        public BookStoreDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
    }
}