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

using static Google.Datastore.V1Beta3.CommitRequest.Types;
using static Google.Datastore.V1Beta3.PropertyFilter.Types;
using static Google.Datastore.V1Beta3.PropertyOrder.Types;
using static Google.Datastore.V1Beta3.ReadOptions.Types;

namespace GoogleCloudSamples.Models
{
    public static class DatastoreBookStoreExtensionMethods
    {
        /// <summary>
        /// Create a datastore entity with the same values as book.
        /// </summary>
        /// <param name="book">The book to store in datastore.</param>
        /// <returns>A datastore entity.</returns>
        /// [START toentity]
        public static Entity ToEntity(this Book book)
        {
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
            // Having to call ?.StringValue is annoying.  In C++, the Value type would
            // be automatically castable to these other types:
            // class Value {
            //   operator string()();
            // Not sure if that's possible in C#.
            // An alternative might be:
            //   book.Title = entity.GetString("Title");
            // Not sure I like it better.
            book.Title = entity["Title"]?.StringValue;
            book.Author = entity["Author"]?.StringValue;
            // TimestampValue doesn't seem very useful.
            book.PublishedDate = entity["PublishedDate"]?.TimestampValue.ToDateTime();
            book.ImageUrl = entity["ImageUrl"]?.StringValue;
            book.Description = entity["Description"]?.StringValue;
            book.CreatedById = entity["CreatedById"]?.StringValue;
            return book;
        }
    }

    public class DatastoreBookStore : IBookStore
    {
        private readonly string _projectId;
        private readonly DatastoreClient _datastore;

        /// <summary>
        /// Create a new datastore-backed bookstore.
        /// </summary>
        /// <param name="projectId">Your Google Cloud project id</param>
        public DatastoreBookStore(string projectId)
        {
            _projectId = projectId;
            // Use Application Default Credentials.
            _datastore = DatastoreClient.Create();
        }

        // [START create]
        public void Create(Book book)
        {
            // Why do I have to pass the _projectId again here if it's already in the key?
            // Actually, I want the projectId baked into the datastore client so I never have
            // to specify it again.
            // Calling .ToInsert() is very weird.
            // Having two very different ways to insert, update, etc. depending on whether or not
            // I'm in a transaction is annoying.  I want one interface to do it.
            // How about a NullTransaction where all operations are immediately committed, and the
            // final .Commit() is a no-op?
            CommitResponse response = _datastore.Commit(_projectId, Mode.NonTransactional, new[] { book.ToEntity().ToInsert() });
            Key key = response.MutationResults[0].Key;
            book.Id = key.Path.First().Id;
        }
        // [END create]

        public void Delete(long id)
        {
            CommitMutation(new Mutation()
            {
                Delete = new Key[] { id.ToKey() }
            });
        }

        // [START list]
        public BookList List(int pageSize, string nextPageToken)
        {
            var query = new Query()
            {
                Limit = pageSize,
                Kinds = new[] { new KindExpression() { Name = "Book" } },
            };

            if (!string.IsNullOrWhiteSpace(nextPageToken))
                query.StartCursor = nextPageToken;

            var datastoreRequest = _datastore.Datasets.RunQuery(
                datasetId: _projectId,
                body: new RunQueryRequest() { Query = query }
            );

            var response = datastoreRequest.Execute();
            var results = response.Batch.EntityResults;
            var books = results.Select(result => result.Entity.ToBook());

            return new BookList()
            {
                Books = books,
                NextPageToken = books.Count() == pageSize
                    && response.Batch.MoreResults == "MORE_RESULTS_AFTER_LIMIT"
                    ? response.Batch.EndCursor : null,
            };
        }
        // [END list]

        public Book Read(long id)
        {
            var found = _datastore.Datasets.Lookup(new LookupRequest()
            {
                Keys = new Key[] { id.ToKey() }
            }, _projectId).Execute().Found;
            if (found == null || found.Count == 0)
            {
                return null;
            }
            return found[0].Entity.ToBook();
        }

        public void Update(Book book)
        {
            CommitMutation(new Mutation()
            {
                Update = new Entity[] { book.ToEntity() }
            });
        }
    }
}
