using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.TechnicalSpecifications;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class CarSpecBuilder
    {
        private readonly CarTechnicalSpecification _carSpec;

        public CarSpecBuilder()
        {
            _carSpec = new CarTechnicalSpecification();
        }

        public CarSpecBuilder WithName(string name)
        {
            _carSpec.Name = name;
            return this;
        }



        public CarSpecBuilder WithId(Guid ID)
        {
            _carSpec.ID = ID;
            return this;
        }


        public CarTechnicalSpecification Build()
        {
            return _carSpec;
        }
    }
}