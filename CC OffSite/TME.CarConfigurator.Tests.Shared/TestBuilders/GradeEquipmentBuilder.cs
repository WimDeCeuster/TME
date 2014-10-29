using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class GradeEquipmentBuilder
    {
        private readonly GradeEquipment _gradeEquipment;

        public GradeEquipmentBuilder()
        {
            _gradeEquipment = new GradeEquipment
            {
                Accessories = new List<GradeAccessory>(),
                Options = new List<GradeOption>()
            };
        }

        public GradeEquipmentBuilder WithAccessories(params GradeAccessory[] accessories)
        {
            _gradeEquipment.Accessories = accessories;

            return this;
        }

        public GradeEquipmentBuilder WithOptions(params GradeOption[] options)
        {
            _gradeEquipment.Options = options;

            return this;
        }

        public GradeEquipment Build()
        {
            return _gradeEquipment;
        }
    }
}