using System;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher
{
    public class ContextData : IContextData
    {
        public IRepository<Car> Cars { get; private set; }
        public IRepository<Generation> Generations { get; private set; }
        public IRepository<String> Strings { get; private set; }

        public ContextData()
        {
            Cars = new Repository<Car>();
            Generations = new Repository<Generation>();
        }
    }
}
