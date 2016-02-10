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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;

using Google.Apis.Storage.v1;
using Google.Apis.Storage.v1.ClientWrapper;

namespace GoogleCloudSamples.Services
{
    public class ImageUploader
    {
        private string _bucketName;
        private string _applicationName;
  
        public ImageUploader(string bucketName, string applicationName)
        {
            _bucketName = bucketName;
            _applicationName = applicationName;
        }

        public async Task<String> UploadImage(IFormFile image, long id)
        {
            // Create client and use it to upload object to Cloud Storage
            var client = await StorageClient
                .FromApplicationCredentials(_applicationName);

            var imageAcl = ObjectsResource
                .InsertMediaUpload.PredefinedAclEnum.PublicRead;

            var imageObject = await client.UploadObjectAsync(
                bucket: _bucketName,
                objectName: id.ToString(),
                contentType: image.ContentType,
                source: image.OpenReadStream(),
                options: new UploadObjectOptions { PredefinedAcl = imageAcl }
            );

            return imageObject.MediaLink;
        }

        public async Task DeleteUploadedImage(long id)
        {
            var client = await StorageClient
                .FromApplicationCredentials(_applicationName);
            client.DeleteObject(_bucketName, id.ToString());
        }
    }
}
