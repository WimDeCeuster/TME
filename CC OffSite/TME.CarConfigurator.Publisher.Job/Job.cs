using TME.CarConfigurator.Publisher.Interfaces;

namespace TME.CarConfigurator.Publisher.Job
{
    internal class Job : IJob
    {
        public ICarConfiguratorPublisher CarConfiguratorPublisher { get; set; }

        public void Run()
        {

        }
    }
}