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

using System;
using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Bookshelf.Models
{
    // [START book]
    [FirestoreData]
    public class Book
    {
        [BindNever]
        public string Id { get; set; }

        [Required]
        [FirestoreProperty]
        public string Title { get; set; }

        [FirestoreProperty]
        public string Author { get; set; }

        [Display(Name = "Date Published")]
        [DataType(DataType.Date)]
        [FirestoreProperty]
        public DateTime? PublishedDate { get; set; }

        [FirestoreProperty]
        public string ImageUrl { get; set; }

        [DataType(DataType.MultilineText)]
        [FirestoreProperty]
        public string Description { get; set; }
    }
    // [END book]
}
