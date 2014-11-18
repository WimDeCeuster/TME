using FakeItEasy;
using TME.CarConfigurator.Factories;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;

namespace TME.CarConfigurator.Query.Tests.TestBuilders
{
    public class EquipmentFactoryBuilder
    {
        private IEquipmentService _equipmentService = A.Fake<IEquipmentService>();
        private IColourFactory _colourFactory = A.Fake<IColourFactory>();
        private IAssetFactory _assetFactory = A.Fake<IAssetFactory>();

        public EquipmentFactoryBuilder WithEquipmentService(IEquipmentService equipmentService)
        {
            _equipmentService = equipmentService;

            return this;
        }

        public EquipmentFactoryBuilder WithColourFactory(IColourFactory colourFactory)
        {
            _colourFactory = colourFactory;

            return this;
        }

        public EquipmentFactoryBuilder WithAssetFactory(IAssetFactory assetFactory)
        {
            _assetFactory = assetFactory;
            return this;
        }

        public IEquipmentFactory Build()
        {
            return new EquipmentFactory(_equipmentService, _colourFactory,_assetFactory);
        }
    }
}