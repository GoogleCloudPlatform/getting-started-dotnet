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

using Google.Datastore.V1Beta3;
using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Google.Api.Gax;

using static Google.Datastore.V1Beta3.CommitRequest.Types;
using static Google.Datastore.V1Beta3.PropertyFilter.Types;
using static Google.Datastore.V1Beta3.PropertyOrder.Types;
using static Google.Datastore.V1Beta3.ReadOptions.Types;
// using Google.Apis.Datastore.v1beta2.Data;

namespace GoogleCloudSamples.Models
{
    public static class DatastoreBookStoreExtensionMethods
    {
        /// <summary>
        /// Make a datastore key given a book's id.
        /// </summary>
        /// <param name="id">A book's id.</param>
        /// <returns>A datastore key.</returns>
        public static Key ToKey(this long id)
        {
            return new Key().WithElement("Book", id);
        }

        /// <summary>
        /// Make a book id given a datastore key.
        /// </summary>
        /// <param name="key">A datastore key</param>
        /// <returns>A book id.</returns>
        public static long ToId(this Key key)
        {
            return (long)key.Path.First().Id;
        }

        // P2 What I really want is an ORM-like solution so that I don't
        // have to implement ToEntity() and ToBook().  It should observe
        // the System.ComponentModel.DataAnnotations annotations and act
        // accordingly.

        /// <summary>
        /// Create a datastore entity with the same values as book.
        /// </summary>
        /// <param name="book">The book to store in datastore.</param>
        /// <returns>A datastore entity.</returns>
        /// [START toentity]
        public static Entity ToEntity(this Book book)
        {
            // Other than the aforementioned ToKey() issues, this is really
            // nice.  About as nice as it can be.
            var entity = new Entity();
            entity.Key = book.Id.ToKey();
            entity["Title"] = book.Title;
            entity["Author"] = book.Author;
            entity["PublishedDate"] = book.PublishedDate;
            entity["ImageUrl"] = book.ImageUrl;
            entity["Description"] = book.Description;
            entity["CreateById"] = book.CreatedById;
            return entity;
        }
        // [END toentity]

        /// <summary>
        /// Unpack a book from a datastore entity.
        /// </summary>
        /// <param name="entity">An entity retrieved from datastore.</param>
        /// <returns>A book.</returns>
        public static Book ToBook(this Entity entity)
        {
            Book book = new Book();
            book.Id = (long)entity.Key.Path.First().Id;
            // P1 Having to call ?.StringValue is annoying.  In C++, the Value type would
            // be automatically castable to these other types:
            // class Value {
            //   operator string()();
            // Not sure if that's possible in C#.
            // An alternative might be:
            //   book.Title = entity<String>["Title"];
            // or
            //   book.Title = entity.Get<String>("Title");
            // or
            //   book.Title = entity.GetString("Title");
            // Not sure I like it better.
            book.Title = (string) entity["Title"];
            book.Author = (string) entity["Author"];
            // P2 TimestampValue doesn't seem very useful.
            book.PublishedDate = (DateTime) entity["PublishedDate"];
            book.ImageUrl = (string) entity["ImageUrl"];
            book.Description = (string) entity["Description"];
            book.CreatedById = (string) entity["CreatedById"];
            return book;
        }
    }

    public class DatastoreBookStore : IBookStore
    {
        private readonly string _projectId;
        private readonly DatastoreClient _datastore;
        private readonly DatastoreFoo _foo;

        /// <summary>
        /// Create a new datastore-backed bookstore.
        /// </summary>
        /// <param name="projectId">Your Google Cloud project id</param>
        public DatastoreBookStore(string projectId)
        {
            _projectId = projectId;
            // Use Application Default Credentials.
            _datastore = DatastoreClient.Create();
            // I like this better.
            _foo = DatastoreFoo.Create(_projectId);
        }

        // [START create]
        public void Create(Book book)
        {
            // P0 Calling .ToInsert() is very weird.
            // Having two very different ways to insert, update, etc. depending on whether or not
            // I'm in a transaction is annoying.  I want one *interface* to do it.
            // How about a NullTransaction where all operations are immediately committed, and the
            // final .Commit() is a no-op?  Or, make DatastoreFoo and Transaction implement the
            // same interface?
            CommitResponse response = _datastore.Commit(_projectId, Mode.NonTransactional, new[] { book.ToEntity().ToInsert() });
            Key key = response.MutationResults[0].Key;
            book.Id = key.Path.First().Id;
        }
        // [END create]

        public void Delete(long id)
        {
            // Pretty good.
            _foo.Delete(id.ToKey());
            var trans = _foo.BeginTransaction();
            trans.Delete(id.ToKey());
            trans.Commit();
        }

        // [START list]
        public BookList List(int pageSize, string nextPageToken)
        {
            var query = new Query("Book")
            {
                Limit = pageSize,
            };

            if (!string.IsNullOrWhiteSpace(nextPageToken))
                query.StartCursor = Google.Protobuf.ByteString.CopyFromUtf8(nextPageToken);

            var response = _foo.RunQuery(query);

            var results = response.Batch.EntityResults;
            var books = results.Select(result => result.Entity.ToBook());

            return new BookList()
            {
                Books = books,
                // More string vs proto byte string warnings below.  Why?
                NextPageToken = books.Count() == pageSize
                    && response.Batch.MoreResults == QueryResultBatch.Types.MoreResultsType.MoreResultsAfterCursor
                    ? response.Batch.EndCursor.ToStringUtf8() : null,
            };

            // List() ends up being still more code than I want to write.
            // Wondering off into the realm of ponies and ORMs, I'd ideally
            // like to write:
            // var query = new Query<Book>() {
            //   ...
            // And have it take care of the entity to Book translations.
        }
        // [END list]

        public Book Read(long id)
        {
            // Nice!
            return _foo.Lookup(id.ToKey())?.ToBook();
        }

        public void Update(Book book)
        {
            // Very nice!
            _foo.Update(book.ToEntity());
        }
    }
}
