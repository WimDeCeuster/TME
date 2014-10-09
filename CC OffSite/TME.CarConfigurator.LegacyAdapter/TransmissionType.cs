using TME.CarConfigurator.Interfaces;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class TransmissionType : BaseObject, ITransmissionType
    {
        #region Dependencies (Adaptee)
        private Legacy.TransmissionType Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public TransmissionType(Legacy.TransmissionType adaptee)
            : base(adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion


    }
}
