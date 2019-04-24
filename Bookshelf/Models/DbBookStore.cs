// Copyright(c) 2016 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Bookshelf.Models
{
    /// <summary>
    /// Implements IBookStore with a database.
    /// </summary>
    public class DbBookStore : IBookStore
    {
        private readonly BookStoreDbContext _dbcontext;

        public DbBookStore(BookStoreDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        // [START create]
        public async Task CreateAsync(Book book)
        {
            book.Id = Guid.NewGuid().ToString();
            var trackBook = await _dbcontext.Books.AddAsync(book);
            await _dbcontext.SaveChangesAsync();
            book.Id = trackBook.Entity.Id;
        }
        // [END create]

        public Task DeleteAsync(string id)
        {
            Book book = new Book { Id = id };
            _dbcontext.Books.Attach(book);
            _dbcontext.Books.Remove(book);
            return _dbcontext.SaveChangesAsync();
        }

        // [START list]
        public async Task<BookList> ListAsync(int pageSize, 
            string nextPageToken)
        {
            IQueryable<Book> query = _dbcontext.Books.OrderBy(book => book.Id);
            if (nextPageToken != null)
            {
                query = query.Where(
                    book => string.CompareOrdinal(book.Id, nextPageToken) > 0);
            }
            var books = await query.Take(pageSize).ToListAsync();
            return new BookList()
            {
                Books = books,
                NextPageToken = books.Count() == pageSize ? 
                    books.Last().Id : null
            };
        }
        // [END list]

        public async Task<Book> ReadAsync(string id)
        {
            return await _dbcontext.Books.FirstAsync(m => m.Id == id);
        }


        public async Task UpdateAsync(Book book)
        {
            _dbcontext.Entry(book).State = EntityState.Modified;
            await _dbcontext.SaveChangesAsync();
        }
    }
}