// Copyright 2015 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
using System.Linq;
using System;
using System.Net;
using Microsoft.AspNet.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using GoogleCloudSamples.Models;

using Google;
using Google.Apis.Storage.v1;
using Google.Apis.Storage.v1.ClientWrapper;
using GoogleCloudSamples.Services;

namespace GoogleCloudSamples.Controllers
{
    public class BooksController : Controller
    {
        /// <summary>
        /// How many books should we display on each page of the index?
        /// </summary>
        private const int _pageSize = 10;
        private IBookStore _store;
        private ImageUploader _imageUploader;

        public BooksController(IBookStore store, ImageUploader imageUploader)
        {
            _store = store;
            _imageUploader = imageUploader;
        }

        // GET: Books
        public IActionResult Index(string nextPageToken)
        {
            return View(new ViewModels.Books.Index() {
                BookList = _store.List(_pageSize, nextPageToken)
            });
        }

        // GET: Books/Details/5
        public IActionResult Details(long? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Book book = _store.Read((long)id);
            if (book == null)
            {
                return HttpNotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return ViewForm("Create", "Create");
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book, 
            [FromForm] IFormFile image)
        {
            if (ModelState.IsValid)
            {
                _store.Create(book);
                // If book cover image submitted, save image to Cloud Storage
                if (image != null)
                {
                    var imageUrl = await _imageUploader.UploadImage(image, book.Id);
                    book.ImageUrl = imageUrl;
                    _store.Update(book);
                }
                return RedirectToAction("Details", new { id = book.Id });
            }
            return ViewForm("Create", "Create", book);
        }
        /// <summary>
        /// Dispays the common form used for the Edit and Create pages.
        /// </summary>
        /// <param name="action">The string to display to the user.</param>
        /// <param name="book">The asp-action value.  Where will the form be 
        /// submitted?</param>
        /// <returns>An IActionResult that displays the form.</returns>
        private IActionResult ViewForm(string action, string formAction, 
            Book book = null)
        {
            var form = new ViewModels.Books.Form()
            {
                Action = action,
                Book = book ?? new Book(),
                IsValid = ModelState.IsValid,
                FormAction = formAction
            };
            return View("/Views/Books/Form.cshtml", form);
        }

        // GET: Books/Edit/5
        public IActionResult Edit(long? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Book book = _store.Read((long)id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return ViewForm("Edit", "Edit", book);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Book book, long id, 
            [FromForm] IFormFile image)
        {
            if (ModelState.IsValid)
            {
                book.Id = id;
                // If book cover image submitted. save image to Cloud Storage
                if (image != null)
                {
                    var imageUrl = await _imageUploader.UploadImage(image, book.Id);
                    book.ImageUrl = imageUrl;
                }
                else
                {
                    //No image submited so set ImageUrl to current stored value
                    book.ImageUrl = _store.Read((long)id).ImageUrl;
                }
                _store.Update(book);
                return RedirectToAction("Details", new { id = book.Id });
            }
            return ViewForm("Edit", "Edit", book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            // Delete book cover image from Cloud Storage if ImageUrl exists
            string imageUrlToDelete = _store.Read((long)id).ImageUrl;
            if (imageUrlToDelete != null)
            {
                await _imageUploader.DeleteUploadedImage(id);
            }
            _store.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
