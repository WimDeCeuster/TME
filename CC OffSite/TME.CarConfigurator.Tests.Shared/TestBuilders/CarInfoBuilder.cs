namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class CarInfoBuilder
    {
        private readonly Repository.Objects.CarInfo _carInfo;

        public CarInfoBuilder()
        {
            _carInfo = new Repository.Objects.CarInfo();
        }

        public CarInfoBuilder WithShortId(int id)
        {
            _carInfo.ShortID = id;

            return this;
        }

        public Repository.Objects.CarInfo Build()
        {
            return _carInfo;
        }
    }
}