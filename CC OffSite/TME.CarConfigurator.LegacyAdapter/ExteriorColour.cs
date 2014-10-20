using TME.CarConfigurator.Interfaces.Colours;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class ExteriorColour : BaseObject, IExteriorColour
    {
        #region Dependencies (Adaptee)
        private Legacy.CarExteriorColour Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public ExteriorColour(Legacy.CarExteriorColour adaptee) : base(adaptee)
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
