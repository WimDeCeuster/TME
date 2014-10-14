using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class EngineTypeBuilder
    {
        private EngineType _engineType;
        public EngineTypeBuilder()
        {
            _engineType = new EngineType();
        }

        public EngineTypeBuilder WithCode(String code)
        {
            _engineType.Code = code;

            return this;
        }

        public EngineTypeBuilder WithFuelType(FuelType fuelType)
        {
            _engineType.FuelType = fuelType;

            return this;
        }

        public EngineType Build()
        {
            return _engineType;
        }
    }
}
