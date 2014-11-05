using System;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.S3.Publisher.Helpers
{
    public class ModeAndView : IEquatable<ModeAndView>
    {
        public string Mode { get; private set; }
        public string View { get; private set; }

        public ModeAndView(AssetType assetType)
        {
            Mode = assetType.Mode;
            View = assetType.View;
        }

        public bool Equals(ModeAndView other)
        {
            return Mode.Equals(other.Mode) && View.Equals(other.View);
        }

        public override int GetHashCode()
        {
            return string.Format("{0}{1}", Mode, View).GetHashCode();
        }
    }
}