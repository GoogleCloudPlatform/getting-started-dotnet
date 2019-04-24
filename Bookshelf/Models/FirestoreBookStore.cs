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

using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task CreateAsync(Book book)
        {
            _logger.LogTrace($"Create {book.Title}");
            DocumentReference docRef = await _books.AddAsync(book);
            book.Id = docRef.Id;
        }

        public Task DeleteAsync(string id)
        {
            _logger.LogTrace($"Delete {id}");
            return _books.Document(id).DeleteAsync();
        }

        public async Task<BookList> ListAsync(int pageSize, string nextPageToken)
        {
            _logger.LogTrace($"List {pageSize}, {nextPageToken}");
            int nextPageStart;
            int.TryParse(nextPageToken, out nextPageStart);
            List<Book> bookList = new List<Book>();
            var snapshot = await _books.Offset(nextPageStart).Limit(pageSize)
                .GetSnapshotAsync();
            foreach (DocumentSnapshot docSnapshot in snapshot.Documents) 
            {
                var book = docSnapshot.ConvertTo<Book>();
                book.Id = docSnapshot.Id;
                bookList.Add(book);
            }
            return new BookList()
            {
                Books = bookList,
                NextPageToken = bookList.Count == pageSize ? 
                    (nextPageStart + pageSize).ToString() : null
            };
        }

        public async Task<Book> ReadAsync(string id)
        {
            _logger.LogTrace($"Read {id}");
            var snapshot = await _books.Document(id).GetSnapshotAsync();
            if (!snapshot.Exists)
            {
                return null;
            }
            Book book = snapshot.ConvertTo<Book>();
            book.Id = snapshot.Id;
            return book;            
        }

        public Task UpdateAsync(Book book)
        {
            _logger.LogTrace($"Update {book.Title}");
            return _books.Document(book.Id).SetAsync(book);
        }
    }
} 