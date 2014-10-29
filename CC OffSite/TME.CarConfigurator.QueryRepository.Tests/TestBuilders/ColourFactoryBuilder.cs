using FakeItEasy;
using TME.CarConfigurator.Factories;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;

namespace TME.CarConfigurator.Query.Tests.TestBuilders
{
    public class ColourFactoryBuilder
    {
        private IColourService _colourService = A.Fake<IColourService>();

        public ColourFactoryBuilder WithColourService(IColourService colourService)
        {
            _colourService = colourService;

            return this;
        }

        public IColourFactory Build()
        {
            return new ColourFactory(_colourService);
        }
    }
}