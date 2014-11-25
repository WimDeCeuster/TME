using System;
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


        public CarSpecBuilder WithSortOrder(int sortOrder)
        {
            _carSpec.SortIndex = sortOrder;
            return this;    
        }

        public CarSpecBuilder WithCategory(CategoryInfo categoryInfo)
        {
            _carSpec.Category = categoryInfo;
            return this;
        }

        public CarTechnicalSpecification Build()
        {
            return _carSpec;
        }
    }
}