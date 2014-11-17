using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces.Colours;

namespace TME.CarConfigurator.Colours
{
    public class ExteriorColourInfo : IExteriorColourInfo
    {
        readonly Repository.Objects.Colours.ExteriorColourInfo _repoInfo;

        public ExteriorColourInfo(Repository.Objects.Colours.ExteriorColourInfo repoInfo)
        {
            if (repoInfo == null) throw new ArgumentNullException("repInfo");

            _repoInfo = repoInfo;
        }

        public Guid ID
        {
            get { return _repoInfo.ID; }
        }

        public string InternalCode
        {
            get { return _repoInfo.InternalCode; }
        }
    }
}
