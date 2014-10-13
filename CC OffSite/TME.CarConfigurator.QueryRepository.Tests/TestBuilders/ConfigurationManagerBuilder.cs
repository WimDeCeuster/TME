using FakeItEasy;
using TME.CarConfigurator.Interfaces.Configuration;

namespace TME.CarConfigurator.Query.Tests.TestBuilders
{
    internal class ConfigurationManagerBuilder
    {
        private readonly IConfigurationManager _configurationManager;

        public ConfigurationManagerBuilder()
        {
            _configurationManager = A.Fake<IConfigurationManager>();

            A.CallTo(() => _configurationManager.DataSubset).Returns("preview");
            A.CallTo(() => _configurationManager.Environment).Returns("development");
            A.CallTo(() => _configurationManager.AmazonAccessKeyId).Returns("access key id");
            A.CallTo(() => _configurationManager.AmazonSecretAccessKey).Returns("secret access key");
        }

        public IConfigurationManager Build()
        {
            return _configurationManager;
        }
    }
}