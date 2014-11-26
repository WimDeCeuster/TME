using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Colours;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class CarExteriorColourBuilder
    {
        readonly CarExteriorColour _colour;

        public CarExteriorColourBuilder()
        {
            _colour = new CarExteriorColour();
        }

        public CarExteriorColourBuilder WithId(Guid id)
        {
            _colour.ID = id;

            return this;
        }

        public CarExteriorColourBuilder WithExteriorColourType(ExteriorColourType type)
        {
            _colour.Type = type;

            return this;
        }

        public CarExteriorColourBuilder AddVisibleIn(String mode, String view)
        {
            if (_colour.VisibleIn == null)
                _colour.VisibleIn = new List<VisibleInModeAndView>();

            _colour.VisibleIn.Add(new VisibleInModeAndView { Mode = mode, View = view, CanHaveAssets = true});

            return this;
        }

        public CarExteriorColour Build()
        {
            return _colour;
        }
    }
}
