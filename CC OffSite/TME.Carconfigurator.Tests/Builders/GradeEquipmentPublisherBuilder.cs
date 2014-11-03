using FakeItEasy;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.S3.Publisher;
using TME.CarConfigurator.S3.Publisher.Helpers;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.Carconfigurator.Tests.Builders
{
    public class EquipmentPublisherBuilder
    {
        private IEquipmentService _service = A.Fake<IEquipmentService>();
        private readonly ITimeFramePublishHelper _timeFramePublishHelper = new TimeFramePublishHelper();

        public EquipmentPublisherBuilder WithService(IEquipmentService service)
        {
            _service = service;

            return this;
        }

        public EquipmentPublisher Build()
        {
            return new EquipmentPublisher(_service, _timeFramePublishHelper);
        }
    }
}
