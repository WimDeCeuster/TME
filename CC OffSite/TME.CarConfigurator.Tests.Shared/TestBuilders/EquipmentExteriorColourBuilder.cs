using System;
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class EquipmentExteriorColourBuilder
    {
        readonly ExteriorColour _colour;

        public EquipmentExteriorColourBuilder()
        {
            _colour = new ExteriorColour();
        }

        public EquipmentExteriorColourBuilder WithId(Guid id)
        {
            _colour.ID = id;

            return this;
        }


        public ExteriorColour Build()
        {
            return _colour;
        }
    }
}
