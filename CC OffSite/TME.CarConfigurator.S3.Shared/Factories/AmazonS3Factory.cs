using Amazon.S3;
using System;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.Shared.Factories
{
    public class AmazonS3Factory : IAmazonS3Factory
    {
        public IAmazonS3 CreateInstance(String accessKey, String secretKey)
        {
            if (String.IsNullOrWhiteSpace(accessKey)) throw new ArgumentNullException("accessKey");
            if (String.IsNullOrWhiteSpace(secretKey)) throw new ArgumentNullException("secretKey");

            return new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.EUWest1);
        }


        public IAmazonS3 CreateInstance()
        {
            return new AmazonS3Client(null, null, Amazon.RegionEndpoint.EUWest1);
        }
    }
}
