using TME.CarConfigurator.Repository.Objects.Core;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.Repository.Objects.Packs;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class CarPackOptionBuilder
    {
        private CarPackOption _option;

        public CarPackOptionBuilder()
        {
            _option = new CarPackOption();
        }

        public CarPackOptionBuilder WithPrice(Price price)
        {
            _option.Price = price;
            return this;
        }

        public CarPackOptionBuilder WithParentOption(OptionInfo parentOption)
        {
            _option.ParentOption = parentOption;
            return this;
        }

        public CarPackOption Build()
        {
            return _option;
        }
    }
}
