using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator
{
    public class FuelType : BaseObject, IFuelType
    {
        private readonly Repository.Objects.FuelType _fuelType;

        public FuelType(Repository.Objects.FuelType fuelType)
            : base(fuelType)
        {
            if (fuelType == null) throw new ArgumentNullException("fuelType");
            _fuelType = fuelType;
        }
        
        public bool Hybrid { get { return _fuelType.Hybrid; } }
    }
}