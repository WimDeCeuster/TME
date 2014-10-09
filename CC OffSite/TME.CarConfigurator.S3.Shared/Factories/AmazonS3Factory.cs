using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.S3.Shared.Factories
{
    public class AmazonS3Factory
    {
        public IAmazonS3 CreateInstance()
        {
            var accessKey = ConfigurationManager.AppSettings["AWSKey"];
            var secretKey = ConfigurationManager.AppSettings["AWSSecretKey"];
            var client = new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.EUWest1);

            return client;
        }
    }
}
