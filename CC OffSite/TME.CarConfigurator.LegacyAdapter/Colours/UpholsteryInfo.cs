using System;
using TME.CarConfigurator.Interfaces.Colours;

namespace TME.CarConfigurator.LegacyAdapter.Colours
{
    public class UpholsteryInfo : IUpholsteryInfo
    {
        public UpholsteryInfo(TMME.CarConfigurator.CarUpholstery carUpholstery)
        {
            ID = carUpholstery.ID;
            InternalCode = carUpholstery.InternalCode;
        }

        public Guid ID { get; private set; }
        public string InternalCode { get; private set; }
    }
}