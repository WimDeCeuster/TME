using System.Collections.Generic;

using TME.CarConfigurator.Interfaces.Assets;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class VisibleInModeAndView : IVisibleInModeAndView
    {
        public string Mode
        {
            get;
            internal set;
        }

        public string View
        {
            get;
            internal set;
        }

        public bool HasMode
        {
            get { return !string.IsNullOrWhiteSpace(Mode); }
        }

        public IEnumerable<IAsset> Assets
        {
            get;
            internal set;
        }
    }
}
