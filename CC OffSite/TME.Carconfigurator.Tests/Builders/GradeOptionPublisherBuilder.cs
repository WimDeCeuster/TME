using FakeItEasy;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.S3.Publisher;
using TME.CarConfigurator.S3.Publisher.Helpers;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.Carconfigurator.Tests.Builders
{
    public class GradeEquipmentPublisherBuilder
    {
        private IGradeEquipmentService _service = A.Fake<IGradeEquipmentService>();
        private ITimeFramePublishHelper _timeFramePublishHelper = new TimeFramePublishHelper();

        public GradeEquipmentPublisherBuilder WithService(IGradeEquipmentService service)
        {
            _service = service;

            return this;
        }

        public GradeEquipmentPublisher Build()
        {
            return new GradeEquipmentPublisher(_service, _timeFramePublishHelper);
        }
    }
}
