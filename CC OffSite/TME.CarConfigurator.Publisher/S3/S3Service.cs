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

namespace TME.CarConfigurator.Publisher.S3
{
    public class S3Service : IService
    {
        AmazonS3Client _client;
        IS3Serialiser _serialiser;
        String _bucketName;
        RegionEndpoint _regionEndPoint = RegionEndpoint.EUWest1;
        String _publicationPathTemplate = "{0}/publication/{1}";
        String _assetPathTemplate = "{0}/publication/{1}/assets";
        String _modelsOverviewPath = "models-per-language";

        public S3Service()
        {
            //throw new NotImplementedException();
        }

        public S3Service(String brand, String country, IS3Serialiser serialiser)
        {
            if (brand == null) throw new ArgumentNullException("brand");
            if (country == null) throw new ArgumentNullException("country");
            if (serialiser == null) throw new ArgumentNullException("serialiser");
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentException("brand cannot be empty", "brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentException("country cannot be empty", "country");

            var accessKey = ConfigurationManager.AppSettings["AWSKey"];
            var secretKey = ConfigurationManager.AppSettings["AWSSecretKey"];
            var bucketNameTemplate = ConfigurationManager.AppSettings["AWSBucketNameTemplate"];
            var environment = ConfigurationManager.AppSettings["Environment"];

            _serialiser = serialiser;
            _client = new AmazonS3Client(accessKey, secretKey, _regionEndPoint);

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

        public T GetObject<T>(String key)
        {
            var response = _client.GetObject(_bucketName, key);

            using (var responseStream = response.ResponseStream)
            using (var reader = new StreamReader(responseStream))
            {
                return _serialiser.Deserialise<T>(reader.ReadToEnd());
            }
        }

        public void DeleteObject(String key)
        {
            _client.DeleteObject(_bucketName, key);
        }

        public Languages GetModelsOverviewPerLanguage()
        {
            try
            { 
                return GetObject<Languages>(_modelsOverviewPath);
            }
            catch (AmazonS3Exception ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return new Languages();
                throw;
            }
        }

        public async Task<Result> PutModelsOverviewPerLanguage(Languages languages)
        {
            return await PutObjectAsync(_modelsOverviewPath, _serialiser.Serialise(languages));
        }

        public async Task<Result> PutPublication(String language, Publication publication)
        {
            if (language == null) throw new ArgumentNullException("language");
            if (publication == null) throw new ArgumentNullException("publication");
            if (String.IsNullOrWhiteSpace(language)) throw new ArgumentException("language cannot empty");

            var path = String.Format(_publicationPathTemplate, language, publication.ID);
            var value = _serialiser.Serialise(publication);

            return await PutObjectAsync(path, value);
        }

        public async Task<Result> PutAssetsOfPublication(string language, Publication publication)
        {
            if (language == null) throw new ArgumentNullException("language");
            if (publication == null) throw new ArgumentNullException("publication");
            if (String.IsNullOrWhiteSpace(language)) throw new ArgumentException("language cannot empty");

            var path = String.Format(_assetPathTemplate, language, publication.ID);
            var value = _serialiser.Serialise(publication.Generation.Assets);

            return await PutObjectAsync(path, value);
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
    }
}