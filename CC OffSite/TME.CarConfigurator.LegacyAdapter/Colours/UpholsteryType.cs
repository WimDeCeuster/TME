using TME.CarConfigurator.Interfaces.Colours;

namespace TME.CarConfigurator.LegacyAdapter.Colours
{
    public class UpholsteryType : BaseObject, IUpholsteryType
    {
        #region Dependencies (Adaptee)
        private TMME.CarConfigurator.UpholsteryType Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public UpholsteryType(TMME.CarConfigurator.UpholsteryType adaptee)
            : base(adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion
    }
}
