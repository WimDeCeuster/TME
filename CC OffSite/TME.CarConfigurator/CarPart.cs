using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;

namespace TME.CarConfigurator
{
    public class CarPart : ICarPart
    {
        private readonly Repository.Objects.CarPart _carPart;

        public CarPart(Repository.Objects.CarPart carPart)
        {
            if (carPart == null) throw new ArgumentNullException("carPart");

            _carPart = carPart;
        }

        public string Code { get { return _carPart.Code; } }
        public string Name { get { return _carPart.Name; } }

        public IReadOnlyList<IVisibleInModeAndView> VisibleIn { get; private set; }
    }
}