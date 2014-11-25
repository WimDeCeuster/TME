using System;
using System.Collections.Generic;

namespace TME.CarConfigurator.Interfaces.Core
{
    public interface IBaseObject
    {
        Guid ID { get;  }
        string InternalCode { get; }
        string LocalCode { get;  }
        string Name { get; }
        string Description { get; }
        string FootNote { get; }
        string ToolTip { get;  }
        int SortIndex { get; }
        IReadOnlyList<ILabel> Labels { get; }
    }
}
