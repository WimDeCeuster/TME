using TME.CarConfigurator.Interfaces.Core;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class Label : ILabel
    {

        
        #region Dependencies (Adaptee)
        private Legacy.Label Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public Label(Legacy.Label adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion


        public string Code
        {
            get { return Adaptee.Code; }
        }

        public string Value
        {
            get { return Adaptee.Value; }
        }
    }
}
