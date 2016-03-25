﻿// Copyright(c) 2016 Google Inc.
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
using System.Web.Mvc;

namespace GoogleCloudSamples.Models
{
    // [START book]
    [Bind(Include = "Title, Author, PublishedDate, Description, ImageUrl")]
    public class Book
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Author { get; set; }

        [Display(Name = "Date Published")]
        [DataType(DataType.Date)]
        public DateTime? PublishedDate { get; set; }

        public string ImageUrl { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public string CreatedById { get; set; }
    }
    // [END book]
}
