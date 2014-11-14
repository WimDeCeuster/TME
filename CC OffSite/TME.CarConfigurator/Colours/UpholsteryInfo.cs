using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces.Colours;

namespace TME.CarConfigurator.Colours
{
    public class UpholsteryInfo : IUpholsteryInfo
    {
        readonly Repository.Objects.Colours.UpholsteryInfo _repoInfo;

        public UpholsteryInfo(Repository.Objects.Colours.UpholsteryInfo repoInfo)
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
