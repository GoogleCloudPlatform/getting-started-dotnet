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

// [START bookshelf_firestore_client]
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
        private FirestoreDb _firestore;
        private CollectionReference _books;

        public FirestoreBookStore(string projectId)
        {
            _firestore = FirestoreDb.Create(projectId);
            _books = _firestore.Collection("Books");
        }
        // [END bookshelf_firestore_client]

        public async Task CreateAsync(Book book)
        {
            // Firestore expects all times to be UTC.
            book.PublishedDate = book.PublishedDate?.ToUniversalTime();
            DocumentReference docRef = await _books.AddAsync(book);
            book.Id = docRef.Id;
        }

        public Task DeleteAsync(string id)
        {
            return _books.Document(id).DeleteAsync();
        }

        public async Task<BookList> ListAsync(int pageSize, string previousBookId)
        {
            List<Book> bookList = new List<Book>();
            var query = _books.OrderBy("Title");
            if (!string.IsNullOrEmpty(previousBookId))
            {
                DocumentSnapshot prevDocSnapshot = await
                    _books.Document(previousBookId).GetSnapshotAsync();
                if (prevDocSnapshot.Exists)
                {
                    query = query.StartAfter(prevDocSnapshot);
                }
            }
            QuerySnapshot snapshot = await query.Limit(pageSize)
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
                    (bookList.Last().Id).ToString() : null
            };
        }

        // [START bookshelf_firestore_client_get_book]
        public async Task<Book> ReadAsync(string id)
        {
            var snapshot = await _books.Document(id).GetSnapshotAsync();
            if (!snapshot.Exists)
            {
                return null;
            }
            Book book = snapshot.ConvertTo<Book>();
            book.Id = snapshot.Id;
            return book;
        }
        // [END bookshelf_firestore_client_get_book]

        public Task UpdateAsync(Book book)
        {
            // Firestore expects all times to be UTC.
            book.PublishedDate = book.PublishedDate?.ToUniversalTime();
            return _books.Document(book.Id).SetAsync(book);
        }
    }
}