using TME.CarConfigurator.Interfaces.Colours;

namespace TME.CarConfigurator.LegacyAdapter.Colours
{
    public class ExteriorColourType : BaseObject, IExteriorColourType
    {
        #region Dependencies (Adaptee)
        private TMME.CarConfigurator.ExteriorColourType Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public ExteriorColourType(TMME.CarConfigurator.ExteriorColourType adaptee)
            : base(adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion
    }
}
