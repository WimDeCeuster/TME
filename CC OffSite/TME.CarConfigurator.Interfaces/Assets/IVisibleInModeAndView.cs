using System.Collections.Generic;

namespace TME.CarConfigurator.Interfaces.Assets
{
    public interface IVisibleInModeAndView
    {
        string Mode { get;  }
        string View { get;  }

        IEnumerable<IAsset> Assets { get; }
    }
}
