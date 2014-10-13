using TME.CarConfigurator.Factories;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;

namespace TME.CarConfigurator.Query.Tests.TestBuilders
{
    public class BodyTypeFactoryBuilder
    {
        private IBodyTypeService _bodyTypeService;
        private IAssetFactory _assetFactory;

        public BodyTypeFactoryBuilder WithBodyTypeService(IBodyTypeService bodyTypeService)
        {
            _bodyTypeService = bodyTypeService;

            return this;
        }

        public BodyTypeFactoryBuilder WithAssetFactory(IAssetFactory assetFactory)
        {
            _assetFactory = assetFactory;

            return this;
        }

        public IBodyTypeFactory Build()
        {
            return new BodyTypeFactory(_bodyTypeService, _assetFactory);
        }
    }
}