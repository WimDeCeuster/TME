using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces.Colours;

namespace TME.CarConfigurator.Colours
{
    public class ColourCombinationInfo : IColourCombinationInfo
    {
        public ColourCombinationInfo(Repository.Objects.Colours.ColourCombinationInfo repoInfo)
        {
            if (repoInfo == null) throw new ArgumentNullException("repoInfo");

            ExteriorColour = new ExteriorColourInfo(repoInfo.ExteriorColour);
            Upholstery = new UpholsteryInfo(repoInfo.Upholstery);
        }

        public IExteriorColourInfo ExteriorColour { get; private set; }
        public IUpholsteryInfo Upholstery { get; private set; }
    }
}
