using FakeItEasy;
using TME.CarConfigurator.Factories;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;

namespace TME.CarConfigurator.Query.Tests.TestBuilders
{
    public class GradeEquipmentFactoryBuilder
    {
        private IGradeEquipmentService _gradeEquipmentService = A.Fake<IGradeEquipmentService>();
        private IColourFactory _colourFactory = A.Fake<IColourFactory>();

        public GradeEquipmentFactoryBuilder WithGradeEquipmentService(IGradeEquipmentService gradeEquipmentService)
        {
            _gradeEquipmentService = gradeEquipmentService;

            return this;
        }

        public GradeEquipmentFactoryBuilder WithColourFactory(IColourFactory colourFactory)
        {
            _colourFactory = colourFactory;

            return this;
        }

        public IGradeEquipmentFactory Build()
        {
            return new GradeEquipmentFactory(_gradeEquipmentService, _colourFactory);
        }
    }
}