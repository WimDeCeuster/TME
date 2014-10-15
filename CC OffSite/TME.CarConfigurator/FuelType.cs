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
        private readonly Repository.Objects.FuelType _repositoryFuelType;

        public FuelType(Repository.Objects.FuelType repositoryFuelType)
            : base(repositoryFuelType)
        {
            if (repositoryFuelType == null) throw new ArgumentNullException("repositoryFuelType");
            _repositoryFuelType = repositoryFuelType;
        }
        
        public bool Hybrid { get { return _repositoryFuelType.Hybrid; } }
    }
}