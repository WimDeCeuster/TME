using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Colours;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class ExteriorColourBuilder
    {
        readonly ExteriorColour _colour;

        public ExteriorColourBuilder()
        {
            _colour = new ExteriorColour();
        }

        public ExteriorColourBuilder WithId(Guid id)
        {
            _colour.ID = id;

            return this;
        }

        public ExteriorColourBuilder WithExteriorColourType(ExteriorColourType type)
        {
            _colour.Type = type;

            return this;
        }

        public ExteriorColourBuilder AddVisibleIn(String mode, String view)
        {
            if (_colour.VisibleIn == null)
                _colour.VisibleIn = new List<VisibleInModeAndView>();

            _colour.VisibleIn.Add(new VisibleInModeAndView { Mode = mode, View = view, CanHaveAssets = true });

            return this;
        }

        public ExteriorColour Build()
        {
            return _colour;
        } 
    }
}