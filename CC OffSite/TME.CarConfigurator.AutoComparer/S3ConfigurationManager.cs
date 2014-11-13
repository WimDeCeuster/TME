using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.S3.Shared.Configuration;

namespace TME.CarConfigurator.AutoComparer
{
    public class S3ConfigurationManager : IConfigurationManager
    {
        public S3ConfigurationManager(Options options)
        {

        }

        public string AmazonAccessKeyId
        {
            get; private set;
        }

        public string AmazonSecretAccessKey
        {
            get; private set;
        }

        public string BucketNameTemplate
        {
            get; private set;
        }
    }
}
