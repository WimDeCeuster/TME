using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using System.Threading.Tasks;
using System.IO;
using System;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.S3.Shared.Exceptions;
using System.Collections.Generic;

namespace TME.CarConfigurator.S3.Shared
{
    public class Service : IService
    {
        IAmazonS3 _client;
        readonly String _bucketNameTemplate;
        readonly HashSet<String> _verifiedBuckets;

        public Service(String bucketNameTemplate, String accessKey, String secretKey, IAmazonS3Factory clientFactory)
        {
            if (clientFactory == null) throw new ArgumentNullException("clientFactory");
            if (String.IsNullOrWhiteSpace(accessKey)) throw new ArgumentNullException("accessKey");
            if (String.IsNullOrWhiteSpace(secretKey)) throw new ArgumentNullException("secretKey");
            if (String.IsNullOrWhiteSpace(secretKey)) throw new ArgumentNullException("bucketNameTemplate");

            _client = clientFactory.CreateInstance(accessKey, secretKey);
            _bucketNameTemplate = bucketNameTemplate;

            _verifiedBuckets = new HashSet<String>();
        }

        public async Task PutObjectAsync(String brand, String country, String key, String item)
        {
            if (brand == null) throw new ArgumentNullException("brand");
            if (country == null) throw new ArgumentNullException("country");
            if (key == null) throw new ArgumentNullException("key");
            if (item == null) throw new ArgumentNullException("item");
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentException("brand cannot be empty", "brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentException("country cannot be empty", "country");
            if (String.IsNullOrWhiteSpace(key)) throw new ArgumentException("key cannot be empty", "key");
            if (String.IsNullOrWhiteSpace(item)) throw new ArgumentException("item cannot be empty", "item");

            var request = new PutObjectRequest
            {
                BucketName = GetBucketName(brand, country),
                Key = key,
                ContentBody = item,
                ContentType = "application/json"
            };

            if (!DoesBucketExist(brand, country))
                CreateBucket(brand, country);

            var response = await _client.PutObjectAsync(request);

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                return;

            throw new PutObjectFailedException(request, response);
        }

        private string GetBucketName(String brand, String country)
        {
            return _bucketNameTemplate.ToLowerInvariant()
                                      .Replace("{brand}", brand.ToLowerInvariant())
                                      .Replace("{country}", country.ToLowerInvariant());
        }

        Boolean DoesBucketExist(String brand, String country)
        {
            if (brand == null) throw new ArgumentNullException("brand");
            if (country == null) throw new ArgumentNullException("country");
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentException("brand cannot be empty", "brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentException("country cannot be empty", "country");

            var bucketName = GetBucketName(brand, country);
            if (_verifiedBuckets.Contains(bucketName))
                return true;

            try
            {
                _client.ListObjects(new ListObjectsRequest
                {
                    BucketName = bucketName,
                    MaxKeys = 0
                });

                _verifiedBuckets.Add(bucketName);

                return true;
            }
            catch (AmazonServiceException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return false;

                throw;
            }
        }

        void CreateBucket(String brand, String country)
        {
            if (brand == null) throw new ArgumentNullException("brand");
            if (country == null) throw new ArgumentNullException("country");
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentException("brand cannot be empty", "brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentException("country cannot be empty", "country");

            _client.PutBucket(new PutBucketRequest
            {
                BucketName = GetBucketName(brand, country),
                UseClientRegion = true
            });
        }

        public String GetObject(String brand, String country, String key)
        {
            if (brand == null) throw new ArgumentNullException("brand");
            if (country == null) throw new ArgumentNullException("country");
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentException("brand cannot be empty", "brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentException("country cannot be empty", "country");

            try
            {
                var bucketName = GetBucketName(brand, country);
                var response = _client.GetObject(bucketName, key);

                using (var responseStream = response.ResponseStream)
                using (var reader = new StreamReader(responseStream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (AmazonS3Exception ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new ObjectNotFoundException(ex, key);
                throw;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Service()
        {
            Dispose(false);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (!disposing || _client == null) return;

            _client.Dispose();
            _client = null;
        }
    }
}