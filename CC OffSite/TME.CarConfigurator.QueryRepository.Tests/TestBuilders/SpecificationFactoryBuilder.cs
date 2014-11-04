using FakeItEasy;
using TME.CarConfigurator.Factories;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;

namespace TME.CarConfigurator.Query.Tests.TestBuilders
{
    public class SpecificationsFactoryBuilder
    {
        private ISpecificationsService _equipmentService = A.Fake<ISpecificationsService>();
        
        public SpecificationsFactoryBuilder WithSpecificationsService(ISpecificationsService equipmentService)
        {
            _equipmentService = equipmentService;

            return this;
        }

        public ISpecificationsFactory Build()
        {
            return new SpecificationsFactory(_equipmentService);
        }
    }
}