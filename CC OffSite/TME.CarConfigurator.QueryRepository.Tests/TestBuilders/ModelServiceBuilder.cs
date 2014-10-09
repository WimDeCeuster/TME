using FakeItEasy;
using TME.CarConfigurator.QueryRepository.Tests.TestBuilders.Base;
using TME.CarConfigurator.S3.QueryServices.Interfaces;

namespace TME.CarConfigurator.QueryRepository.Tests.TestBuilders
{
    internal class ModelServiceBuilder : ServiceBuilderBase<IModelService>
    {
        private readonly IModelService _service;

        private ModelServiceBuilder(IModelService service)
        {
            _service = service;
        }

        public static ModelServiceBuilder InitializeFakeService()
        {
            var service = A.Fake<IModelService>();

            return new ModelServiceBuilder(service);
        }

        public override IModelService Build()
        {
            return _service;
        }
    }
}