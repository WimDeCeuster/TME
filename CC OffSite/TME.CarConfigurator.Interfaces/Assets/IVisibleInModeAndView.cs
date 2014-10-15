using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.Interfaces.Assets
{
    public interface IVisibleInModeAndView
    {
        string Mode { get;  }
        string View { get;  }

        IEnumerable<IAsset> Assets { get; }
    }
}
