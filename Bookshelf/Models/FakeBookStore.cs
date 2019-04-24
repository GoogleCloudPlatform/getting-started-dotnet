// Copyright(c) 2019 Google Inc.
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
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Bookshelf.Models
{
    class FakeBookStore : IBookStore
    {
        private static Book s_fakeBook = new Book() 
        {
            Author = "Herman Melville",
            Title = "Moby Dick",
            Id = "1",
            PublishedDate = DateTime.Parse("1851-01-01"),
            Description = "It's about a whale.",
        };

        private readonly ILogger<FakeBookStore> _logger;

        public FakeBookStore(ILogger<FakeBookStore> logger)
        {
            this._logger = logger;
        }

        public Task CreateAsync(Book book)
        {
            _logger.LogTrace($"Create {book.Title}");
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string id)
        {
            _logger.LogTrace($"Delete {id}");
            return Task.CompletedTask;
        }

        public Task<BookList> ListAsync(int pageSize, string nextPageToken)
        {
            _logger.LogTrace($"List {pageSize}, {nextPageToken}");
            var bookList = new BookList()
            {
                Books = new [] {s_fakeBook}
            };
            return Task.FromResult(bookList);
        }

        public Task<Book> ReadAsync(string id)
        {
            _logger.LogTrace($"Read {id}");
            return Task.FromResult(s_fakeBook);
        }

        public Task UpdateAsync(Book book)
        {
             _logger.LogTrace($"Update {book.Title}");
             return Task.CompletedTask;
        }
    }
}