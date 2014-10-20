using FakeItEasy;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.S3.Publisher;

namespace TME.Carconfigurator.Tests.Builders
{
    public class SubModelPublisherBuilder
    {
        private ISubModelService _subModelService = A.Fake<ISubModelService>();

        public SubModelPublisherBuilder WithService(ISubModelService subModelService)
        {
            _subModelService = subModelService;
            return this;
        }

        public ISubModelPublisher Build()
        {
            return new SubModelPublisher(_subModelService);
        }
    }
}