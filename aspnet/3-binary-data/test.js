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

casper.start(host + '/', function (response) {
    console.log('Starting ' + host + '/');
    this.test.assertEquals(response.status, 302);
});

casper.thenOpen(host + '/Books', function (response) {
    this.test.assertEquals(response.status, 200);
});

casper.thenClick('#add-book', function () {
    console.log('Clicked Add book.  New location is ' + this.getCurrentUrl());
    this.test.assertExists({ type: 'xpath', path: '//input[@type="file"]' },
    'The Form element "image" exists for uploading book cover images .');
    this.fill('form#book-form', {
        'Book.Title': 'test.js',
        'Book.Author': 'test.js',
        'Book.PublishedDate': '2000-01-01',
        'Book.Description': 'Automatically added by test.js'
    }, false);
    console.log('Filled form.');
});

casper.thenClick('button', function () {
    console.log('Submitted.  New location is ' + this.getCurrentUrl());
    this.test.assertEquals(this.fetchText('.book-description'),
        'Automatically added by test.js');
});

casper.thenClick('button', function (response) {
    console.log('Deleted new book.  New location is ' + this.getCurrentUrl());
    this.test.assertEquals(response.status, 200);
});

casper.run(function () {
    this.test.done();
    this.test.renderResults(true);
});
