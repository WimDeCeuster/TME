using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher
{
    public interface IContextData
    {
        IRepository<Car> Cars { get; }
        IRepository<Generation> Generations { get; }
    }

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
