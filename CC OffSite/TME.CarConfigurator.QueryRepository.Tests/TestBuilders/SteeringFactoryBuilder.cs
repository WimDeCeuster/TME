using FakeItEasy;
using TME.CarConfigurator.Factories;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;

namespace TME.CarConfigurator.Query.Tests.TestBuilders
{
    public class SteeringFactoryBuilder
    {
        private ISteeringService _steeringService = A.Fake<ISteeringService>();

        public SteeringFactoryBuilder WithSteeringService(ISteeringService steeringService)
        {
            _steeringService = steeringService;

            return this;
        }

        public ISteeringFactory Build()
        {
            return new SteeringFactory(_steeringService);
        }
    }
}