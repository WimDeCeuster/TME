using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class CarOptionBuilder
    {
        private readonly CarOption _carOption;

        public CarOptionBuilder()
        {
            _carOption = new CarOption();
        }

        public CarOptionBuilder WithId(Guid ID)
        {
            _carOption.ID = ID;
            return this;    
        }

        public CarOptionBuilder WithAvailableForUpholsteries(params Repository.Objects.Colours.UpholsteryInfo[] upholsteryInfos)
        {
            _carOption.AvailableForUpholsteries = upholsteryInfos.ToList();

            return this;
        }

        public CarOptionBuilder WithVisibleIn(string mode, string view)
        {
            if (_carOption.VisibleIn == null)
                _carOption.VisibleIn = new List<VisibleInModeAndView>();
            _carOption.VisibleIn.Add(new VisibleInModeAndView(){Mode = mode, View = view});

            return this;
        }

        public CarOptionBuilder WithAvailableForExteriorColours(params Repository.Objects.Colours.ExteriorColourInfo[] exteriorColourInfos)
        {
            _carOption.AvailableForExteriorColours = exteriorColourInfos.ToList();

            return this;
        }

        public CarOption Build()
        {
            return _carOption;
        }
    }
}