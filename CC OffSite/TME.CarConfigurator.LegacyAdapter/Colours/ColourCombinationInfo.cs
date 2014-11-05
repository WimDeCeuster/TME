using TME.CarConfigurator.Interfaces.Colours;

namespace TME.CarConfigurator.LegacyAdapter.Colours
{
    public class ColourCombinationInfo : IColourCombinationInfo
    {

        #region Dependencies (Adaptee)
        private TMME.CarConfigurator.CarColourCombination Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public ColourCombinationInfo(TMME.CarConfigurator.CarColourCombination adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

        public IExteriorColourInfo ExteriorColour
        {
            get {return new ExteriorColourInfo(Adaptee.ExteriorColour);}
        }
        public IUpholsteryInfo Upholstery
        {
            get { return new UpholsteryInfo(Adaptee.Upholstery); }
        }
    }
}
