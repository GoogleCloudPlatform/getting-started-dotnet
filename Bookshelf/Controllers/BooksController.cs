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

using Bookshelf.Models;
using Bookshelf.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Web;

namespace Bookshelf.Controllers
{
    public class BooksController : Controller
    {
        /// <summary>
        /// How many books should we display on each page of the index?
        /// </summary>
        private const int _pageSize = 10;

        private readonly IBookStore _store;
        private readonly ImageUploader _imageUploader;

        public BooksController(IBookStore store, ImageUploader imageUploader)
        {
            _store = store;
            _imageUploader = imageUploader;
        }

        // GET: Books
        public async Task<IActionResult> Index(string nextPageToken)
        {
            return View(new ViewModels.Books.Index()
            {
                BookList = await _store.ListAsync(_pageSize, nextPageToken)
            });
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Book book = await _store.ReadAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return ViewForm("Create", "Create");
        }

        // [START create]
        // POST: Books/Create
        [HttpPost]
        public async Task<IActionResult> Create(Book book, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                await _store.CreateAsync(book);
                // If book cover image submitted, save image to Cloud Storage
                if (image != null)
                {
                    var imageUrl = await _imageUploader.UploadImage(image, book.Id);
                    book.ImageUrl = imageUrl;
                    await _store.UpdateAsync(book);
                }
                return RedirectToAction("Details", new { id = book.Id });
            }
            return ViewForm("Create", "Create", book);
        }
        // [END create]

        /// <summary>
        /// Dispays the common form used for the Edit and Create pages.
        /// </summary>
        /// <param name="action">The string to display to the user.</param>
        /// <param name="book">The asp-action value.  Where will the form be submitted?</param>
        /// <returns>An IIActionResult that displays the form.</returns>
        private IActionResult ViewForm(string action, string formAction, Book book = null)
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
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Book book = await _store.ReadAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return ViewForm("Edit", "Edit", book);
        }

        // POST: Books/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(Book book, string id, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                book.Id = id;
                if (image != null)
                {
                    book.ImageUrl = await _imageUploader.UploadImage(image, book.Id);
                }
                await _store.UpdateAsync(book);
                return RedirectToAction("Details", new { id = book.Id });
            }
            return ViewForm("Edit", "Edit", book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            // Delete book cover image from Cloud Storage if ImageUrl exists
            string imageUrlToDelete = (await _store.ReadAsync(id)).ImageUrl;
            if (imageUrlToDelete != null)
            {
                await _imageUploader.DeleteUploadedImage(id);
            }
            await _store.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}