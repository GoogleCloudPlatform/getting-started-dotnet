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
using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;

namespace Bookshelf.Models
{
    class FirestoreBookStore : IBookStore
    {
        private readonly ILogger<FirestoreBookStore> _logger;
        private FirestoreDb _firestore;
        private CollectionReference _books;

        public FirestoreBookStore(ILogger<FirestoreBookStore> logger)
        {
            this._logger = logger;
            _firestore = FirestoreDb.Create();
            _books = _firestore.Collection("Books");
        }

        public void Create(Book book)
        {
            _logger.LogTrace($"Create {book.Title}");
            DocumentReference docRef = _books.AddAsync(book).Result;
            book.Id = docRef.Id;
        }

        public void Delete(string id)
        {
            _logger.LogTrace($"Delete {id}");
            _books.Document(id).DeleteAsync().Wait();
        }

        public BookList List(int pageSize, string nextPageToken)
        {
            _logger.LogTrace($"List {pageSize}, {nextPageToken}");
            return new BookList()
            {
                Books = _books.GetSnapshotAsync().Result.`
            };
        }

        public Book Read(string id)
        {
            _logger.LogTrace($"Read {id}");
            return s_fakeBook;
        }

        public void Update(Book book)
        {
             _logger.LogTrace($"Update {book.Title}");
        }

        Document BookToDoc(Book book)
        {
            Document doc = new Document() 
            {
                Fields = {"a": Value.}
            }

        }
    }
}