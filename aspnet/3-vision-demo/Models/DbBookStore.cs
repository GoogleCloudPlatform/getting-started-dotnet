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

using System.Linq;

namespace GoogleCloudSamples.Models
{
    /// <summary>
    /// Implements IBookStore with a database.
    /// </summary>
    public class DbBookStore : IBookStore
    {
        private readonly ApplicationDbContext _dbcontext;

        public DbBookStore(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        // [START create]
        public void Create(Book book)
        {
            var trackBook = _dbcontext.Books.Add(book);
            _dbcontext.SaveChanges();
            book.Id = trackBook.Id;
        }
        // [END create]
        public void Delete(long id)
        {
            Book book = _dbcontext.Books.Single(m => m.Id == id);
            _dbcontext.Books.Remove(book);
            _dbcontext.SaveChanges();
        }

        // [START list]
        public BookList List(int pageSize, string nextPageToken)
        {
            IQueryable<Book> query = _dbcontext.Books.OrderBy(book => book.Id);
            if (nextPageToken != null)
            {
                long previousBookId = long.Parse(nextPageToken);
                query = query.Where(book => book.Id > previousBookId);
            }
            var books = query.Take(pageSize).ToArray();
            return new BookList()
            {
                Books = books,
                NextPageToken = books.Count() == pageSize ? books.Last().Id.ToString() : null
            };
        }
        // [END list]

        public Book Read(long id)
        {
            return _dbcontext.Books.Single(m => m.Id == id);
        }

        public void Update(Book book)
        {
            _dbcontext.Entry(book).State = System.Data.Entity.EntityState.Modified;
            _dbcontext.SaveChanges();
        }
    }
}