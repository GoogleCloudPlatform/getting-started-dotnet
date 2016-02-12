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
namespace GoogleCloudSamples.ViewModels.Books
{
    public class Form
    {
        /// <summary>
        /// The book to be displayed in the form.
        /// </summary>
        public Models.Book Book;
        /// <summary>
        /// The string displayed to the user.  Either "Edit" or "Create".
        /// </summary>
        public string Action;
        /// <summary>
        /// False when the user tried entering a bad field value.  For example, they entered
        /// "yesterday" for Date Published.
        /// </summary>
        public bool IsValid;
        /// <summary>
        ///  The target of submit form.  Fills asp-action="".
        /// </summary>
        public string FormAction;
    }
}
