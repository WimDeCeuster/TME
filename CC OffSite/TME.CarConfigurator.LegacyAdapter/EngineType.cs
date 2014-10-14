using System;
using TME.CarConfigurator.Interfaces;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class EngineType : IEngineType
    {
        #region Dependencies (Adaptee)
        private Legacy.Engine Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public EngineType(Legacy.Engine adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

        public string Code
        {
            get { return "NA"; }
        }

        public string Name
        {
            get { return Adaptee.EngineTypeDescription; }
        }

        public IFuelType FuelType
        {
            get { return new FuelType(Adaptee.Type);}
        }
    }
}
