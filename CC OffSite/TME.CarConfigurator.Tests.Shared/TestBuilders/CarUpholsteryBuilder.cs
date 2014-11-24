using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Colours;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class CarUpholsteryBuilder
    {
        readonly CarUpholstery _colour;

        public CarUpholsteryBuilder()
        {
            _colour = new CarUpholstery();
        }

        public CarUpholsteryBuilder WithId(Guid id)
        {
            _colour.ID = id;

            return this;
        }

        public CarUpholsteryBuilder WithExteriorColourType(UpholsteryType type)
        {
            _colour.Type = type;

            return this;
        }

        public CarUpholsteryBuilder AddVisibleIn(String mode, String view)
        {
            if (_colour.VisibleIn == null)
                _colour.VisibleIn = new List<VisibleInModeAndView>();

            _colour.VisibleIn.Add(new VisibleInModeAndView { Mode = mode, View = view });

            return this;
        }

        public CarUpholstery Build()
        {
            return _colour;
        }
    }
}