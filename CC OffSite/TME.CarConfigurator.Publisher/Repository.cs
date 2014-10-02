using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.Publisher
{
    public interface IRepository<T> : IList<T>
    {

    }

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
