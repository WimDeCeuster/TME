using System;
using TME.CarConfigurator.Interfaces.Colours;

namespace TME.CarConfigurator.LegacyAdapter.Colours
{
    public class ExteriorColourInfo : IExteriorColourInfo
    {
        public ExteriorColourInfo(CarExteriorColour carExteriorColour)
        {
            ID = carExteriorColour.ID;
            InternalCode = carExteriorColour.InternalCode;
        }

        public Guid ID { get; private set; }
        public string InternalCode { get; private set; }
    }
}