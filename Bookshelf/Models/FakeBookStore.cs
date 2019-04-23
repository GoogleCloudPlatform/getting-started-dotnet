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
using Microsoft.Extensions.Logging;

namespace Bookshelf.Models
{
    class FakeBookStore : IBookStore
    {
        private static Book s_fakeBook = new Book() 
        {
            Author = "Herman Melville",
            Title = "Moby Dick",
            Id = 1,
            PublishedDate = DateTime.Parse("1851-01-01"),
            Description = "It's about a whale.",
        };

        private readonly ILogger<FakeBookStore> _logger;

        public FakeBookStore(ILogger<FakeBookStore> logger)
        {
            this._logger = logger;
        }

        public void Create(Book book)
        {
            _logger.LogTrace($"Create {book.Title}");
        }

        public void Delete(long id)
        {
            _logger.LogTrace($"Delete {id}");
        }

        public BookList List(int pageSize, string nextPageToken)
        {
            _logger.LogTrace($"List {pageSize}, {nextPageToken}");
            return new BookList()
            {
                Books = new [] {s_fakeBook}
            };
        }

        public Book Read(long id)
        {
            _logger.LogTrace($"Read {id}");
            return s_fakeBook;
        }

        public void Update(Book book)
        {
             _logger.LogTrace($"Update {book.Title}");
       }
    }
}