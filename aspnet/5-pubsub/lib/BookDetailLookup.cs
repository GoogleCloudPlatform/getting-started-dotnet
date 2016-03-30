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
using Google.Apis.Pubsub.v1;
using Google.Apis.Pubsub.v1.Data;
using Google.Apis.Services;
using GoogleCloudSamples.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleCloudSamples
{
    /// <summary>
    /// A library for the background task of looking up a book in Google's books API.
    /// </summary>
    public class BookDetailLookup
    {
        private readonly PubsubService _pubsub;
        private readonly string _topicPath;
        private readonly string _subscriptionPath;

        /// <summary>
        /// We json-encode this message and publish it to the topic.
        /// </summary>
        // [START queuemessage]
        private class QueueMessage
        {
            public long BookId;
        };
        // [END queuemessage]

        public class Options
        {
            public string TopicName = "book-process-queue";
            public string SubscriptionName = "shared-worker-subscription";
        };

        public BookDetailLookup(string projectId, Options options = null)
        {
            options = options ?? new Options();
            // [START pubsubpaths]
            _topicPath = $"projects/{projectId}/topics/{options.TopicName}";
            _subscriptionPath = $"projects/{projectId}/subscriptions/{options.SubscriptionName}";
            // [END pubsubpaths]
            var credentials = Google.Apis.Auth.OAuth2.GoogleCredential.GetApplicationDefaultAsync()
                .Result;
            if (credentials.IsCreateScopedRequired)
            {
                credentials = credentials.CreateScoped(new[] { PubsubService.Scope.Pubsub });
            }
            _pubsub = new PubsubService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
            });
        }

        // Using a sophisticated logger like log4net is beyond the scope of this sample.
        private class ConsoleLogger
        {
            public void LogVerbose(string message) => Debug.WriteLine(message);

            public void LogError(string message, Exception e) => Debug.WriteLine(message);
        };

        private ConsoleLogger Logger => new ConsoleLogger();

        /// <summary>
        /// Creates the topic and subscription, if they don't already exist.  You should call this
        /// once at the beginning of your app.
        /// </summary>
        // [START createtopicandsubscription]
        public void CreateTopicAndSubscription()
        {
            try
            {
                _pubsub.Projects.Topics.Create(new Topic() { Name = _topicPath }, _topicPath)
                    .Execute();
                Logger.LogVerbose("Created topic " + _topicPath);
            }
            catch (Google.GoogleApiException e)
            {
                // A 409 is ok.  It means the topic already exists.
                if (e.Error.Code != 409)
                    throw;
            }
            try
            {
                _pubsub.Projects.Subscriptions.Create(new Subscription()
                {
                    Name = _subscriptionPath,
                    Topic = _topicPath
                }, _subscriptionPath).Execute();
                Logger.LogVerbose("Created subscription " + _subscriptionPath);
            }
            catch (Google.GoogleApiException e)
            {
                // A 409 is ok.  It means the subscription already exists.
                if (e.Error.Code != 409)
                    throw;
            }
        }
        // [END createtopicandsubscription]

        public Task StartPullLoop(IBookStore bookStore, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => PullLoop((long bookId) =>
            {
                Logger.LogVerbose($"Processing {bookId}.");
                ProcessBook(bookStore, bookId);
            }, cancellationToken));
        }

        /// <summary>
        /// Pulls books from the queue and invokes your callback for each book.
        /// </summary>
        /// <param name="callback">This callback will be invoked for every book.
        /// Must be thread safe.</param>
        /// <param name="cancellationToken">Stop looking by cancelling this token.
        /// Must not be null.</param>
        public void PullLoop(Action<long> callback, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    PullOnce(callback, cancellationToken);
                }
                catch (Exception e)
                {
                    Logger.LogError("PullOnce() failed.", e);
                }
            }
        }

        /// <summary>
        /// Makes one call to PubSub.Pull to pull some books from the subscription.
        /// </summary>
        // [START pullonce]
        private void PullOnce(Action<long> callback, CancellationToken cancellationToken)
        {
            Logger.LogVerbose("Pulling messages from subscription...");
            // Pull some messages from the subscription.
            var response = _pubsub.Projects.Subscriptions.Pull(new PullRequest()
            {
                MaxMessages = 3,
                ReturnImmediately = false
            }, _subscriptionPath).ExecuteAsync(cancellationToken).Result;
            if (response.ReceivedMessages == null)
            {
                // HTTP Request expired because the queue was empty.  Ok.
                Logger.LogVerbose("Pulled no messages.");
                return;
            }
            Logger.LogVerbose($"Pulled {response.ReceivedMessages.Count} messages.");
            foreach (var message in response.ReceivedMessages)
            {
                try
                {
                    // Unpack the message.
                    byte[] json = Convert.FromBase64String(message.Message.Data);
                    var qmessage = JsonConvert.DeserializeObject<QueueMessage>(
                        Encoding.UTF8.GetString(json));
                    // Invoke ProcessBook().
                    callback(qmessage.BookId);
                }
                catch (Exception e)
                {
                    Logger.LogError("Error processing book.", e);
                }
            }
            // Acknowledge the message so we don't see it again.
            var ackIds = new string[response.ReceivedMessages.Count];
            for (int i = 0; i < response.ReceivedMessages.Count; ++i)
                ackIds[i] = response.ReceivedMessages[i].AckId;
            _pubsub.Projects.Subscriptions.Acknowledge(new AcknowledgeRequest() { AckIds = ackIds },
                _subscriptionPath).Execute();
        }
        // [END pullonce]

        /// <summary>
        /// Publish a message asking for a book to be processed.
        /// </summary>
        // [START enqueuebook]
        public void EnqueueBook(long bookId)
        {
            var message = new QueueMessage() { BookId = bookId };
            byte[] json = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            string base64 = Convert.ToBase64String(json);

            _pubsub.Projects.Topics.Publish(new PublishRequest()
            {
                Messages = new[] { new PubsubMessage() { Data = base64 } },
            }, _topicPath).Execute();
        }
        // [END enqueuebook]

        /// <summary>
        /// Look up a book in Google's Books API.  Update the book in the book store.
        /// </summary>
        /// <param name="bookStore">Where the book is stored.</param>
        /// <param name="bookId">The id of the book to look up.</param>
        /// <returns></returns>
        // [START processbook]
        public static void ProcessBook(IBookStore bookStore, long bookId)
        {
            var book = bookStore.Read(bookId);
            var query = "https://www.googleapis.com/books/v1/volumes?q="
                + Uri.EscapeDataString(book.Title);
            var response = WebRequest.Create(query).GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            var json = reader.ReadToEnd();
            UpdateBookFromJson(json, book);
            bookStore.Update(book);
        }
        // [END processbook]

        /// <summary>
        /// Parse a date time.  Return null if it can't be parsed.  A single number will be 
        /// interpreted as a year, and the date returned will be YEAR-01-01.
        /// </summary>
        /// <param name="dateString">A string representation of the date.</param>
        /// <returns>A datetime or null.</returns>
        public static DateTime? ParseDate(string dateString)
        {
            DateTime result;
            if (DateTime.TryParse(dateString, out result))
                return result;
            int year;
            if (int.TryParse(dateString, out year))
                return new DateTime(year, 1, 1);
            return null;
        }

        /// <summary>
        /// Updates book with information parsed from a json response from Google's Books API.
        /// </summary>
        /// <param name="json">A response from Google's Books API.</param>
        /// <param name="book">Fields in book will be overwritten.</param>
        public static void UpdateBookFromJson(string json, Book book)
        {
            // There are many volumeInfos, and many are incomplete.  So, to find a field like
            // "title", we use LINQ to scan multiple volumeInfos.
            JObject results = JObject.Parse(json);
            if (results.Property("items") == null)
                return;
            var infos = results["items"].Select(token => (JObject)token)
                .Where(obj => obj.Property("volumeInfo") != null)
                .Select(obj => obj["volumeInfo"]);
            Func<string, IEnumerable<JToken>> GetInfo = (propertyName) =>
            {
                return infos.Select(token => (JObject)token)
                    .Where(info => info.Property(propertyName) != null)
                    .Select(info => info[propertyName]);
            };
            foreach (var title in GetInfo("title").Take(1))
                book.Title = title.ToString();
            // Find the oldest publishedDate.
            var publishedDates = GetInfo("publishedDate")
                .Select(value => ParseDate(value.ToString()))
                .Where(date => date != null)
                .OrderBy(date => date);
            foreach (var date in publishedDates.Take(1))
                book.PublishedDate = date;
            foreach (var authors in GetInfo("authors").Take(1))
                book.Author = string.Join(", ", authors.Select(author => author.ToString()));
            foreach (var description in GetInfo("description").Take(1))
                book.Description = description.ToString();
            foreach (JObject imageLinks in GetInfo("imageLinks"))
            {
                if (imageLinks.Property("thumbnail") != null)
                {
                    book.ImageUrl = imageLinks["thumbnail"].ToString();
                    break;
                }
            }
        }
    }
}