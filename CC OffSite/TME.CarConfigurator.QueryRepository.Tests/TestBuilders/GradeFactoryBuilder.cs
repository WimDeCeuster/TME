using FakeItEasy;
using TME.CarConfigurator.Factories;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;

namespace TME.CarConfigurator.Query.Tests.TestBuilders
{
    public class GradeFactoryBuilder
    {
        private IGradeService _gradeService = A.Fake<IGradeService>();
        private IAssetFactory _assetFactory = A.Fake<IAssetFactory>();
        private IGradeEquipmentFactory _gradeEquipmentFactory =  A.Fake<IGradeEquipmentFactory>();

        public GradeFactoryBuilder WithGradeService(IGradeService gradeService)
        {
            _gradeService = gradeService;

            return this;
        }

        public GradeFactoryBuilder WithAssetFactory(IAssetFactory assetFactory)
        {
            _assetFactory = assetFactory;

            return this;
        }

        public GradeFactoryBuilder WithGradeEquipmentFactory(IGradeEquipmentFactory gradeEquipmentFactory)
        {
            _gradeEquipmentFactory = gradeEquipmentFactory;

            return this;
        }

        public IGradeFactory Build()
        {
            return new GradeFactory(_gradeService, _assetFactory, _gradeEquipmentFactory);
        }
    }
}