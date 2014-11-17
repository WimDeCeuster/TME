using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class CarPartBuilder
    {
        private CarPart _carPart;

        public CarPartBuilder()
        {
            _carPart = new CarPart();
        }

        public CarPartBuilder WithName(string name)
        {
            _carPart.Name = name;
            return this;
        }

        public CarPartBuilder WithCode(string code)
        {
            _carPart.Code = code;
            return this;
        }

        public CarPartBuilder WithId(Guid ID)
        {
            _carPart.ID = ID;
            return this;
        }

        public CarPartBuilder AddVisibleIn(string mode, string view)
        {
            if (_carPart.VisibleIn == null)
                _carPart.VisibleIn = new List<VisibleInModeAndView>();

            _carPart.VisibleIn.Add(new VisibleInModeAndView() {Mode = mode, View = view});
            return this;
        }

        public CarPart Build()
        {
            return _carPart;
        }
    }
}