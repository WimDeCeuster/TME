using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using TME.CarConfigurator.Publisher.Interfaces;
using System;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Publisher.Enums.Result;
using TME.CarConfigurator.Publisher.S3.Exceptions;

namespace TME.CarConfigurator.Publisher.S3
{
    public class S3Service : IS3Service
    {
        IAmazonS3 _client;
        readonly String _bucketName;

        public S3Service(String brand, String country, IAmazonS3 client)
        {
            if (brand == null) throw new ArgumentNullException("brand");
            if (country == null) throw new ArgumentNullException("country");
            if (client == null) throw new ArgumentNullException("client");
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentException("brand cannot be empty", "brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentException("country cannot be empty", "country");

            //var accessKey = ConfigurationManager.AppSettings["AWSKey"];
            //var secretKey = ConfigurationManager.AppSettings["AWSSecretKey"];
            _client = client; //new AmazonS3Client(accessKey, secretKey, _regionEndPoint);


            var bucketNameTemplate = ConfigurationManager.AppSettings["AWSBucketNameTemplate"];
            var environment = ConfigurationManager.AppSettings["Environment"];

            _bucketName = bucketNameTemplate.Replace("{environment}", environment.ToLowerInvariant())
                                            .Replace("{brand}", brand.ToLowerInvariant())
                                            .Replace("{country}", country.ToLowerInvariant());
        }

        public async Task<Result> PutObjectAsync(String key, String item)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (item == null) throw new ArgumentNullException("item");
            if (String.IsNullOrWhiteSpace(key)) throw new ArgumentException("key cannot be empty", "key");
            if (String.IsNullOrWhiteSpace(item)) throw new ArgumentException("item cannot be empty", "item");

            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
                ContentBody = item,
                ContentType = "application/json"
            };

            if (!DoesBucketExist())
                CreateBucket();

            var result = await _client.PutObjectAsync(request);

            if (result.HttpStatusCode == System.Net.HttpStatusCode.OK)
                return new Successfull();
            else
                return new Failed();
        }

        Boolean DoesBucketExist()
        {
            try
            {
                _client.ListObjects(new ListObjectsRequest
                {
                    BucketName = _bucketName,
                    MaxKeys = 0
                });

                return true;
            }
            catch (AmazonServiceException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return false;

                throw;
            }
        }

        void CreateBucket()
        {
            _client.PutBucket(new PutBucketRequest
            {
                BucketName = _bucketName,
                UseClientRegion = true
            });
        }

        public String GetObject(String key)
        {
            try { 
                var response = _client.GetObject(_bucketName, key);

                using (var responseStream = response.ResponseStream)
                using (var reader = new StreamReader(responseStream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Amazon.S3.AmazonS3Exception ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new ObjectNotFoundException(ex);
                throw;
            }
        }

        public void DeleteObject(String key)
        {
            _client.DeleteObject(_bucketName, key);
        }

        internal List<S3Bucket> GetBuckets()
        {
            return _client.ListBuckets().Buckets;
        }

        internal List<S3Object> GetObjects()
        {
            return _client.ListObjects(_bucketName).S3Objects;
        }

        internal void DeleteAll()
        {
            var request = new DeleteObjectsRequest {BucketName = _bucketName};
            foreach (var x in GetObjects())
                request.AddKey(x.Key);

            _client.DeleteObjects(request);
        }

        internal async Task<Int32> GetObjectsAsync()
        {
            var result = await _client.ListObjectsAsync(new ListObjectsRequest { BucketName = _bucketName });

            return result.S3Objects.Count;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~S3Service()
        {
            Dispose(false);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                if (_client != null)
                {
                    _client.Dispose();
                    _client = null;
                }
            }
        }
    }
}