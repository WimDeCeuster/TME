using FakeItEasy;
using TME.CarConfigurator.Factories;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;

namespace TME.CarConfigurator.Query.Tests.TestBuilders
{
    public class GradeEquipmentFactoryBuilder
    {
        private IGradeEquipmentService _gradeEquipmentService = A.Fake<IGradeEquipmentService>();

        public GradeEquipmentFactoryBuilder WithGradeEquipmentService(IGradeEquipmentService gradeEquipmentService)
        {
            _gradeEquipmentService = gradeEquipmentService;

            return this;
        }

        public IGradeEquipmentFactory Build()
        {
            return new GradeEquipmentFactory(_gradeEquipmentService);
        }
    }
}