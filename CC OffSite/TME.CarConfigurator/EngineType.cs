using System;
using TME.CarConfigurator.Interfaces;

namespace TME.CarConfigurator
{
    public class EngineType : IEngineType
    {
        private readonly Repository.Objects.EngineType _repositoryEngineType;
        private IFuelType _fuelType;

        public EngineType(Repository.Objects.EngineType repositoryEngineType)
        {
            if (repositoryEngineType == null) throw new ArgumentNullException("repositoryEngineType");
            _repositoryEngineType = repositoryEngineType;
        }
        public string Code { get { return _repositoryEngineType.Code; } }
        public string Name { get { return _repositoryEngineType.Name; } }
        public IFuelType FuelType { get { return _fuelType = _fuelType ?? new FuelType(_repositoryEngineType.FuelType); } }
    }
}