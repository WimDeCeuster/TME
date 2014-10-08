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
using TME.CarConfigurator.Publisher.Factories;

namespace TME.CarConfigurator.Publisher.S3
{
    public class S3Service : IS3Service
    {
        const String BucketNameTemplate = "{environment}-cardb-{brand}-{country}";
        readonly String _environment;
        IAmazonS3 _client;

        public S3Service(IAmazonS3 client)
        {
            _client = client ?? new AmazonS3Factory().CreateInstance();
            _environment = ConfigurationManager.AppSettings["Environment"];
        }

        public async Task<Result> PutObjectAsync(String brand, String country, String key, String item)
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

            var result = await _client.PutObjectAsync(request);

            if (result.HttpStatusCode == System.Net.HttpStatusCode.OK)
                return new Successfull();
            else
                return new Failed();
        }

        private string GetBucketName(String brand, String country)
        {
            return BucketNameTemplate.Replace("{environment}", _environment.ToLowerInvariant())
                                     .Replace("{brand}", brand.ToLowerInvariant())
                                     .Replace("{country}", country.ToLowerInvariant());
        }

        Boolean DoesBucketExist(String brand, String country)
        {
            if (brand == null) throw new ArgumentNullException("brand");
            if (country == null) throw new ArgumentNullException("country");
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentException("brand cannot be empty", "brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentException("country cannot be empty", "country");
            
            try
            {
                _client.ListObjects(new ListObjectsRequest
                {
                    BucketName = GetBucketName(brand, country),
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
                var response = _client.GetObject(GetBucketName(brand, country), key);

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

        public void DeleteObject(String brand, String country, String key)
        {
            if (brand == null) throw new ArgumentNullException("brand");
            if (country == null) throw new ArgumentNullException("country");
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentException("brand cannot be empty", "brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentException("country cannot be empty", "country");

            _client.DeleteObject(GetBucketName(brand, country), key);
        }

        internal List<S3Bucket> GetBuckets()
        {
            return _client.ListBuckets().Buckets;
        }

        internal List<S3Object> GetObjects(String brand, String country)
        {
            if (brand == null) throw new ArgumentNullException("brand");
            if (country == null) throw new ArgumentNullException("country");
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentException("brand cannot be empty", "brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentException("country cannot be empty", "country");

            return _client.ListObjects(GetBucketName(brand, country)).S3Objects;
        }

        internal void DeleteAll(String brand, String country)
        {
            if (brand == null) throw new ArgumentNullException("brand");
            if (country == null) throw new ArgumentNullException("country");
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentException("brand cannot be empty", "brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentException("country cannot be empty", "country");

            var request = new DeleteObjectsRequest { BucketName = GetBucketName(brand, country) };
            foreach (var x in GetObjects(brand, country))
                request.AddKey(x.Key);

            _client.DeleteObjects(request);
        }

        internal async Task<Int32> GetObjectsAsync(String brand, String country)
        {
            if (brand == null) throw new ArgumentNullException("brand");
            if (country == null) throw new ArgumentNullException("country");
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentException("brand cannot be empty", "brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentException("country cannot be empty", "country");

            var result = await _client.ListObjectsAsync(new ListObjectsRequest { BucketName = GetBucketName(brand, country) });

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