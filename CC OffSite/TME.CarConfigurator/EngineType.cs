using System;
using TME.CarConfigurator.Interfaces;

namespace TME.CarConfigurator
{
    public class EngineType : IEngineType
    {
        private readonly Repository.Objects.EngineType _engineType;
        private IFuelType _fuelType;

        public EngineType(Repository.Objects.EngineType engineType)
        {
            if (engineType == null) throw new ArgumentNullException("engineType");
            _engineType = engineType;
        }
        public string Code { get { return _engineType.Code; } }
        public string Name { get { return _engineType.Name; } }
        public IFuelType FuelType { get { return _fuelType = _fuelType ?? new FuelType(_engineType.FuelType); } }
    }
}