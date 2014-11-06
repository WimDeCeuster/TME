using System.Collections.Generic;

namespace TME.CarConfigurator.Interfaces.Assets
{
    public interface IVisibleInModeAndView
    {
        string Mode { get;  }
        string View { get;  }

        IReadOnlyList<IAsset> Assets { get; }
    }
}
