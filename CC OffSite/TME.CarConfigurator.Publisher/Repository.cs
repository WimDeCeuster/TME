using System.Collections.Generic;
using TME.CarConfigurator.Publisher.Interfaces;

namespace TME.CarConfigurator.Publisher
{
    public class Repository<T> : List<T>, IRepository<T>
    {
        public Repository()
            : base()
        {

        }

        public Repository(IEnumerable<T> collection)
            : base(collection)
        {

        }
    }
}
