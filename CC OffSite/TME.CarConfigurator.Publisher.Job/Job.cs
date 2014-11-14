using System;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Interfaces;

namespace TME.CarConfigurator.Publisher.Job
{
    internal class Job : IJob
    {
        public ICarConfiguratorPublisher CarConfiguratorPublisher { get; set; }
        public IConfigurationManager ConfigurationManager { get; set; }

        public async Task Run()
        {
            var configuration = ConfigurationManager.LoadConfiguration();

            foreach (var countryCode in configuration.Countries)
            {
                await PublishAllModels(countryCode, configuration);
            }
        }

        private async Task PublishAllModels(string countryCode, Configuration configuration)
        {
            MyContext.SetSystemContext(configuration.Brand, countryCode, "en");

            var isPreview = configuration.Datasubset == PublicationDataSubset.Preview;

            var models = Models.GetModels();

            var generations = models
                .Where(m => isPreview
                    ? m.Preview && m.Generations.Any(g => g.Preview)
                    : m.Approved && m.Generations.Any(g => g.Approved))
                .Select(m => m.Generations.Single(g => isPreview ? g.Preview : g.Approved));

            Console.WriteLine("{0}\n--",countryCode.ToUpper());

            foreach (var generation in generations)
            {
                Console.Write(generation.Name);

                var startTime = DateTime.Now;

                // these calls cannot be made in parallel, because then not all models will be activated because of concurrency
                await CarConfiguratorPublisher.PublishAsync(
                    generation.ID,
                    configuration.Environment,
                    configuration.Target,
                    configuration.Brand,
                    countryCode,
                    configuration.Datasubset,
                    configuration.PublishedBy,
                    null);

                Console.WriteLine(" - {0}", DateTime.Now.Subtract(startTime).ToString("h'h 'm'm 's's'"));
            }
        }
    }
}