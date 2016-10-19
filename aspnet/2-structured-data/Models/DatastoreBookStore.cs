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

using Google.Datastore.V1;
using Google.Protobuf;
using System;
using System.Linq;

namespace GoogleCloudSamples.Models
{
    public static class DatastoreBookStoreExtensionMethods
    {
        /// <summary>
        /// Make a datastore key given a book's id.
        /// </summary>
        /// <param name="id">A book's id.</param>
        /// <returns>A datastore key.</returns>
        public static Key ToKey(this long id) =>
            new Key().WithElement("Book", id);

        /// <summary>
        /// Make a book id given a datastore key.
        /// </summary>
        /// <param name="key">A datastore key</param>
        /// <returns>A book id.</returns>
        public static long ToId(this Key key) => key.Path.First().Id;

        /// <summary>
        /// Create a datastore entity with the same values as book.
        /// </summary>
        /// <param name="book">The book to store in datastore.</param>
        /// <returns>A datastore entity.</returns>
        /// [START toentity]
        public static Entity ToEntity(this Book book) => new Entity()
        {
            Key = book.Id.ToKey(),
            ["Title"] = book.Title,
            ["Author"] = book.Author,
            ["PublishedDate"] = book.PublishedDate?.ToUniversalTime(),
            ["ImageUrl"] = book.ImageUrl,
            ["Description"] = book.Description,
            ["CreateById"] = book.CreatedById
        };
        // [END toentity]

        /// <summary>
        /// Unpack a book from a datastore entity.
        /// </summary>
        /// <param name="entity">An entity retrieved from datastore.</param>
        /// <returns>A book.</returns>
        public static Book ToBook(this Entity entity) => new Book()
        {
            Id = entity.Key.Path.First().Id,
            Title = (string)entity["Title"],
            Author = (string)entity["Author"],
            PublishedDate = (DateTime?)entity["PublishedDate"],
            ImageUrl = (string)entity["ImageUrl"],
            Description = (string)entity["Description"],
            CreatedById = (string)entity["CreatedById"]
        };
    }

    public class DatastoreBookStore : IBookStore
    {
        private readonly string _projectId;
        private readonly DatastoreDb _db;

        /// <summary>
        /// Create a new datastore-backed bookstore.
        /// </summary>
        /// <param name="projectId">Your Google Cloud project id</param>
        public DatastoreBookStore(string projectId)
        {
            _projectId = projectId;
            _db = DatastoreDb.Create(_projectId);
        }

        // [START create]
        public void Create(Book book)
        {
            var entity = book.ToEntity();
            entity.Key = _db.CreateKeyFactory("Book").CreateIncompleteKey();
            var keys = _db.Insert(new[] { entity });
            book.Id = keys.First().Path.First().Id;
        }
        // [END create]

        public void Delete(long id)
        {
            _db.Delete(id.ToKey());
        }

        // [START list]
        public BookList List(int pageSize, string nextPageToken)
        {
            var query = new Query("Book") { Limit = pageSize };
            if (!string.IsNullOrWhiteSpace(nextPageToken))
                query.StartCursor = ByteString.FromBase64(nextPageToken);
            var results = _db.RunQuery(query);
            return new BookList()
            {
                Books = results.Entities.Select(entity => entity.ToBook()),
                NextPageToken = results.Entities.Count == query.Limit ?
                    results.EndCursor.ToBase64() : null
            };
        }
        // [END list]

        public Book Read(long id)
        {
            return _db.Lookup(id.ToKey())?.ToBook();
        }

        public void Update(Book book)
        {
            _db.Update(book.ToEntity());
        }
    }
}