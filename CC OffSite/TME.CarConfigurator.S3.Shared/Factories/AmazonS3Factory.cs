using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.Shared.Factories
{
    public class AmazonS3Factory : IAmazonS3Factory
    {
        public IAmazonS3 CreateInstance(String accessKey, String secretKey)
        {
            if (accessKey == null) throw new ArgumentNullException("acccessKey");
            if (secretKey == null) throw new ArgumentNullException("secretKey");
            if (String.IsNullOrWhiteSpace(accessKey)) throw new ArgumentException("accessKey cannot be empty");
            if (String.IsNullOrWhiteSpace(secretKey)) throw new ArgumentException("secretKey cannot be empty");

            var client = new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.EUWest1);

            return client;
        }


        public IAmazonS3 CreateInstance()
        {
            return new AmazonS3Client(null, null, Amazon.RegionEndpoint.EUWest1);
        }
    }
}
