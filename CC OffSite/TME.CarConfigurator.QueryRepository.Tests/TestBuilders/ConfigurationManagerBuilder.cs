using FakeItEasy;
using TME.CarConfigurator.S3.Shared.Configuration;

namespace TME.CarConfigurator.Query.Tests.TestBuilders
{
    internal class ConfigurationManagerBuilder
    {
        private readonly IConfigurationManager _configurationManager;

        public ConfigurationManagerBuilder()
        {
            _configurationManager = A.Fake<IConfigurationManager>();

            A.CallTo(() => _configurationManager.BucketNameTemplate).Returns("bucket name template");
            A.CallTo(() => _configurationManager.AmazonAccessKeyId).Returns("access key id");
            A.CallTo(() => _configurationManager.AmazonSecretAccessKey).Returns("secret access key");
        }

        public IConfigurationManager Build()
        {
            return _configurationManager;
        }
    }
}