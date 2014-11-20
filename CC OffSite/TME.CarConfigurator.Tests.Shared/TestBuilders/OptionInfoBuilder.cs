using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class OptionInfoBuilder
    {
        private OptionInfo _info;

        public OptionInfoBuilder()
        {
            _info = new OptionInfo();
        }

        public OptionInfoBuilder WithId(int id)
        {
            _info.ShortID = id;
            return this;
        }

        public OptionInfo Build()
        {
            return _info;
        }
    }
}
