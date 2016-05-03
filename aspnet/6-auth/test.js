// Copyright(c) 2015 Google Inc.
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

// 1.0 style test script not using the `casperjs test` subcommand
var casper = require('casper').create();
var host = casper.cli.args[0];
var fileName = './cat-image.jpg';
var placeholderImageUrl = 'http://placekitten.com/g/128/192';
var imageSrcUrl = '';
var bookDetailsUrl = '';

casper.start(host + '/', function (response) {
    console.log('Starting ' + host + '/');
    this.test.assertEquals(response.status, 302);
});

casper.thenOpen(host + '/Books', function (response) {
    this.test.assertEquals(response.status, 200);
    this.test.assertExists({ type: 'xpath', path: "//a[text()='Login']" });
});

casper.thenClick('#add-book', function () {
    console.log('Clicked Add book.  New location is ' + this.getCurrentUrl());
    this.test.assertExists({ type: 'xpath', path: '//input[@type="file"]' },
    'The Form element "image" exists for uploading book cover images .');
    this.fill('form#book-form', {
        'Book.Title': 'test.js',
        'Book.Author': 'test.js',
        'Book.PublishedDate': '2000-01-01',
        'Book.Description': 'Automatically added by test.js',
        'image': fileName
    }, false);
    console.log('Filled form.');
});

casper.thenClick('button', function () {
    console.log('Submitted.  New location is ' + this.getCurrentUrl());
    this.test.assertEquals(this.fetchText('.book-description'),
        'Automatically added by test.js');
    // Save the current URL for revisiting, after testing cover image
    bookDetailsUrl = this.getCurrentUrl();
    // Confirm that cover image src isn't set to placeholder image
    imageSrcUrl = this.getElementAttribute('img[class="book-image"]', 'src');
    this.test.assertNotEquals(imageSrcUrl, placeholderImageUrl,
        'Image src is not set to placeholder image.')
    console.log('Image src = ' + imageSrcUrl);
});

casper.thenOpen(imageSrcUrl, function (response) {
    console.log('Testing for 200 HTTP status of cover image URL.');
    this.test.assertEquals(response.status, 200);
});

casper.thenOpen(bookDetailsUrl, function (response) {
    console.log('Reloading book details page.');
    this.test.assertEquals(response.status, 200);
});

casper.thenClick('button', function (response) {
    console.log('Deleted new book.  New location is ' + this.getCurrentUrl());
    this.test.assertEquals(response.status, 200);
});

casper.run(function () {
    this.test.done();
    this.test.renderResults(true);
});
