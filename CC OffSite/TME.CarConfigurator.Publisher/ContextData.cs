using System;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher
{
    public class ContextData
    {
        public IRepository<Car> Cars { get; private set; }
        public IRepository<Generation> Generations { get; private set; }
        public IRepository<Model> Models { get; private set; }

        public ContextData()
        {
            Cars = new Repository<Car>();
            Models = new Repository<Model>();
            Generations = new Repository<Generation>();
            Models = new Repository<Model>();
        }
    }
}
