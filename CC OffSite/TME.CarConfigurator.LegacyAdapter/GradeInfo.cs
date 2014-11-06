using System;
using TME.CarConfigurator.Interfaces;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class GradeInfo : IGradeInfo
    {
       #region Dependencies (Adaptee)
        private Legacy.Grade Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public GradeInfo(Legacy.Grade adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

        public Guid ID
        {
            get { return Adaptee.ID; }
        }

        public string Name
        {
            get { return Adaptee.Name; }
        }
    }
}