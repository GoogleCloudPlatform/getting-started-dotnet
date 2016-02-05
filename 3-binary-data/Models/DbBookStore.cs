// Copyright 2015 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
using System;
using System.Linq;

namespace GoogleCloudSamples.Models
{
    /// <summary>
    /// Implements IBookStore with a database.
    /// </summary>
    public class DbBookStore : IBookStore
    {
        private ApplicationDbContext _dbcontext;
        public DbBookStore(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public void Create(Book book)
        {
            var trackBook = _dbcontext.Books.Add(book);
            _dbcontext.SaveChanges();
            book.Id = trackBook.Entity.Id;
        }

        public void Delete(long id)
        {
            Book book = _dbcontext.Books.Single(m => m.Id == id);
            _dbcontext.Books.Remove(book);
            _dbcontext.SaveChanges();
        }

        public BookList List(int pageSize, string nextPageToken)
        {
            var pageOfBooks = (null == nextPageToken ?
                (from book in _dbcontext.Books orderby book.Id select book) :
                (from book in _dbcontext.Books
                 where book.Id > Int32.Parse(nextPageToken)
                 orderby book.Id
                 select book)).Take(pageSize + 1);
            var bookArray = pageOfBooks.ToArray();
            return new BookList()
            {
                Books = bookArray.Take(pageSize),
                NextPageToken = bookArray.Count() > pageSize ?
                    bookArray[pageSize - 1].Id.ToString() : null
            };
        }

        public Book Read(long id)
        {
            return _dbcontext.Books.Single(m => m.Id == id);
        }

        public void Update(Book book)
        {
            _dbcontext.Update(book);
            _dbcontext.SaveChanges();
        }
    }
}
