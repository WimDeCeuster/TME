using TME.CarConfigurator.Interfaces;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class Generation : BaseObject, IGeneration
    {
        #region Dependencies (Adaptee)
        private Legacy.Generation Adaptee { get; set; }
        #endregion

        #region Constructor
        public Generation(Legacy.Generation adaptee) 
            : base(adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion
    }
}