using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class CarPartBuilder
    {
        private CarPart _carPart;

        public CarPartBuilder()
        {
            _carPart = new CarPart();
        }

        public CarPartBuilder WithName(string name)
        {
            _carPart.Name = name;
            return this;
        }

        public CarPartBuilder WithCode(string code)
        {
            _carPart.Code = code;
            return this;
        }

        public CarPart Build()
        {
            return _carPart;
        }
    }
}