using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface IEngineFactory
    {
        IEnumerable<IEngine> GetEngines(Publication publication, Context context);
    }
}
