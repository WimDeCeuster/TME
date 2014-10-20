using TME.CarConfigurator.Interfaces.Colours;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class ColourTransformation : IColourTransformation
    {
        #region Dependencies (Adaptee)

        private TMME.CarConfigurator.ColourTransformation Adaptee
        {
            get;
            set;
        }

        #endregion

        #region Constructor

        public ColourTransformation(TMME.CarConfigurator.ColourTransformation adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

        public string RGB
        {
            get { return Adaptee.RGB; }
        }

        public decimal Hue
        {
            get { return Adaptee.Hue; }
        }

        public decimal Saturation
        {
            get { return Adaptee.Saturation; }
        }

        public decimal Brightness
        {
            get { return Adaptee.Brightness; }
        }

        public decimal Contrast
        {
            get { return Adaptee.Contrast; }
        }

        public decimal Alpha
        {
            get { return Adaptee.Alpha; }
        }
    }
}