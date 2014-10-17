using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;

namespace TME.CarConfigurator
{
    public class WheelDrive : BaseObject, IWheelDrive
    {
        readonly Repository.Objects.WheelDrive _repositoryWheelDrive;

        public WheelDrive(Repository.Objects.WheelDrive repositoryWheelDrive)
            : base(repositoryWheelDrive)
        {
            if (repositoryWheelDrive == null) throw new ArgumentNullException("repositoryWheelDrive");

            _repositoryWheelDrive = repositoryWheelDrive;
        }

        public Boolean KeyFeature { get { return _repositoryWheelDrive.KeyFeature; } }

        public Boolean Brochure { get { return _repositoryWheelDrive.Brochure; } }

        public IEnumerable<IVisibleInModeAndView> VisibleIn { get { return new List<IVisibleInModeAndView>(); } }

        public IEnumerable<IAsset> Assets { get { return new List<IAsset>(); } }

        [Obsolete("Use the new VisibleIn property instead")]
        public Boolean VisibleInExteriorSpin { get { return false; } }
        [Obsolete("Use the new VisibleIn property instead")]
        public Boolean VisibleInInteriorSpin { get { return false; } }
        [Obsolete("Use the new VisibleIn property instead")]
        public Boolean VisibleInXRay4X4Spin { get { return false; } }
        [Obsolete("Use the new VisibleIn property instead")]
        public Boolean VisibleInXRayHybridSpin { get { return false; } }
        [Obsolete("Use the new VisibleIn property instead")]
        public Boolean VisibleInXRaySafetySpin { get { return false; } }
    }
}
