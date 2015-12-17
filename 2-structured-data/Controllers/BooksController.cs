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
using Microsoft.AspNet.Mvc;
using GoogleCloudSamples.Models;

namespace GoogleCloudSamples.Controllers
{
    public class BooksController : Controller
    {
        /// <summary>
        /// How many books should we display on each page of the index?
        /// </summary>
        private const int pageSize = 10;
        private IBookStore _store;

        public BooksController(IBookStore store)
        {
            _store = store;
        }

        // GET: Books
        public IActionResult Index(string nextPageToken)
        {
            return View(_store.List(pageSize, nextPageToken));
        }

        // GET: Books/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Book book = _store.Read((int)id);
            if (book == null)
            {
                return HttpNotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return ViewForm(ViewModels.Books.Form.Action.CREATE);
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Book book)
        {
            if (ModelState.IsValid)
            {
                _store.Create(book);
                return RedirectToAction("Details",
                     new { id = book.Id });
            }
            return ViewForm(ViewModels.Books.Form.Action.CREATE, book);
        }
        /// <summary>
        /// Dispays the common form used for the Edit and Create pages.
        /// </summary>
        /// <returns>An IActionResult that displays the form.</returns>
        private IActionResult ViewForm(ViewModels.Books.Form.Action action,
            Book book = null)
        {
            var form = new ViewModels.Books.Form()
            {
                action = action,
                book = book ?? new Book()
            };
            return View("/Views/Books/Form.cshtml", form);
        }

        // GET: Books/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Book book = _store.Read((int)id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return ViewForm(ViewModels.Books.Form.Action.EDIT, book);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Book book)
        {
            if (ModelState.IsValid)
            {
                _store.Update(book);
                return RedirectToAction("Details", new { id = book.Id });
            }
            return ViewForm(ViewModels.Books.Form.Action.EDIT, book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _store.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
