using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class FuelTypeBuilder
    {
        private  FuelType _fuelType;
        public FuelTypeBuilder()
        {
            _fuelType = new FuelType();
        }

        public FuelTypeBuilder WithId(Guid id)
        {
            _fuelType.ID = id;

            return this;
        }

        public FuelTypeBuilder WithLabels(params Repository.Objects.Core.Label[] labels)
        {
            _fuelType.Labels = labels.ToList();

            return this;
        }

        public FuelType Build()
        {
            return _fuelType;
        }
    }
}
