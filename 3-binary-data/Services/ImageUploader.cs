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
