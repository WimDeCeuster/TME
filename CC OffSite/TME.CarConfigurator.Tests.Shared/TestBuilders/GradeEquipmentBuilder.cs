using System;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class GradeEquipmentBuilder
    {
        private readonly GradeEquipment _gradeEquipment;

        public GradeEquipmentBuilder()
        {
            _gradeEquipment = new GradeEquipment();
        }

        public GradeEquipment Build()
        {
            return _gradeEquipment;
        }
    }
}