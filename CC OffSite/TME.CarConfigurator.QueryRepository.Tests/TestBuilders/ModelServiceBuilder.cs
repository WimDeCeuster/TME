using FakeItEasy;
using TME.CarConfigurator.Query.Tests.TestBuilders.Base;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.S3.QueryServices;

namespace TME.CarConfigurator.Query.Tests.TestBuilders
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

        public static ModelServiceBuilder Initialize()
        {
            return new ModelServiceBuilder(null);
        }

        public override IModelService Build()
        {
            return _service ?? new ModelService(Serialiser, Service, KeyManager);
        }
    }
}