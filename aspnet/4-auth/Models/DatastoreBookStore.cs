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

using Google.Apis.Datastore.v1beta2;
using Google.Apis.Datastore.v1beta2.Data;
using System;
using System.Collections.Generic;
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
        public static Key ToKey(this long id)
        {
            return new Key()
            {
                Path = new KeyPathElement[]
                {
                    new KeyPathElement() { Kind = "Book", Id = (id == 0 ? (long?)null : id) }
                }
            };
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

        /// <summary>
        /// Get the property from the dict and return null if it isn't there.
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Property GetValue(this IDictionary<string, Property> properties, string key)
        {
            Property value;
            bool found = properties.TryGetValue(key, out value);
            return found ? value : null;
        }

        /// <summary>
        /// Creates a new property iff value is not null.
        /// </summary>
        public static Property NewProperty(string value)
        {
            return null == value ? null : new Property() { StringValue = value };
        }

        public static Property NewProperty(DateTime? value)
        {
            return null == value ? null : new Property() { DateTimeValue = value };
        }

        /// <summary>
        /// Create a datastore entity with the same values as book.
        /// </summary>
        /// <param name="book">The book to store in datastore.</param>
        /// <returns>A datastore entity.</returns>
        /// [START toentity]
        public static Entity ToEntity(this Book book)
        {
            // TODO: Use reflection so we don't have to modify the code every time we add or drop
            // a property from Book.
            var entity = new Entity();
            entity.Properties =
                new Dictionary<string, Property>();
            entity.Key = book.Id.ToKey();
            entity.Properties["Title"] = NewProperty(book.Title);
            entity.Properties["Author"] = NewProperty(book.Author);
            entity.Properties["PublishedDate"] = NewProperty(book.PublishedDate);
            entity.Properties["ImageUrl"] = NewProperty(book.ImageUrl);
            entity.Properties["Description"] = NewProperty(book.Description);
            entity.Properties["CreatedById"] = NewProperty(book.CreatedById);
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
            // TODO: Use reflection so we don't have to modify the code every time we add or drop
            // a property from Book.
            Book book = new Book();
            book.Id = (long)entity.Key.Path.First().Id;
            book.Title = entity.Properties.GetValue("Title")?.StringValue;
            book.Author = entity.Properties.GetValue("Author")?.StringValue;
            book.PublishedDate = entity.Properties.GetValue("PublishedDate")?.DateTimeValue;
            book.ImageUrl = entity.Properties.GetValue("ImageUrl")?.StringValue;
            book.Description = entity.Properties.GetValue("Description")?.StringValue;
            book.CreatedById = entity.Properties.GetValue("CreatedById")?.StringValue;
            return book;
        }
    }

    public class DatastoreBookStore : IBookStore
    {
        private readonly string _projectId;
        private readonly DatastoreService _datastore;

        /// <summary>
        /// Create a new datastore-backed bookstore.
        /// </summary>
        /// <param name="projectId">Your Google Cloud project id</param>
        public DatastoreBookStore(string projectId)
        {
            _projectId = projectId;
            // Use Application Default Credentials.
            var credentials = Google.Apis.Auth.OAuth2.GoogleCredential
                .GetApplicationDefaultAsync().Result;
            credentials = credentials.CreateScoped(new[] {
                DatastoreService.Scope.Datastore
            });
            // Create our connection to datastore.
            _datastore = new DatastoreService(new Google.Apis.Services
                .BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
            });
        }

        /// <summary>
        /// A convenience function which commits a mutation to datastore.
        /// Use this function to avoid a lot of boilerplate.
        /// </summary>
        /// <param name="mutation">The change to make to datastore.</param>
        /// <returns>The result of commiting the change.</returns>
        // [START commitmutation]
        private CommitResponse CommitMutation(Mutation mutation)
        {
            var commitRequest = new CommitRequest()
            {
                Mode = "NON_TRANSACTIONAL",
                Mutation = mutation
            };
            return _datastore.Datasets.Commit(commitRequest, _projectId)
                .Execute();
        }
        // [END commitmutation]

        // [START create]
        public void Create(Book book)
        {
            var result = CommitMutation(new Mutation()
            {
                InsertAutoId = new Entity[] { book.ToEntity() }
            });
            book.Id = result.MutationResult.InsertAutoIdKeys.First().Path.First().Id.Value;
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
        public BookList List(int pageSize, string nextPageToken, string userId = null)
        {
            var query = new Query()
            {
                Limit = pageSize,
                Kinds = new[] { new KindExpression() { Name = "Book" } },
            };

            if (!string.IsNullOrWhiteSpace(nextPageToken))
                query.StartCursor = nextPageToken;

            if (userId != null)
            {
                // Query for books created by the user
                query.Filter = new Filter()
                {
                    PropertyFilter = new PropertyFilter()
                    {
                        Property = new PropertyReference() { Name = "CreatedById" },
                        Operator__ = "equal",
                        Value = new Value() { StringValue = userId }
                    }
                };
            }

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
