using TME.CarConfigurator.Interfaces.Colours;

namespace TME.CarConfigurator.LegacyAdapter.Colours
{
    public class ExteriorColour : BaseObject, IExteriorColour
    {
        #region Dependencies (Adaptee)
        private TMME.CarConfigurator.CarExteriorColour Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public ExteriorColour(TMME.CarConfigurator.CarExteriorColour adaptee) : base(adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

        public IColourTransformation Transformation
        {
            get { return new ColourTransformation(Adaptee.Transformation);}
        }
    }
}
