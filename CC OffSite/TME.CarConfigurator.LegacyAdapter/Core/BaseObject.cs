using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces.Core;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public abstract class BaseObject : IBaseObject
    {

        #region Dependencies (Adaptee)
        private Legacy.BaseObject Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        protected BaseObject(Legacy.BaseObject adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion
        public Guid ID
        {
            get { return Adaptee.ID; }
        }

        public string InternalCode
        {
            get { return Adaptee.InternalCode; }
        }

        public string LocalCode
        {
            get { return Adaptee.LocalCode; }
        }

        public string Name
        {
            get { return Adaptee.Name; }
        }

        public string Description
        {
            get { return Adaptee.Description; }
        }

        public string FootNote
        {
            get { return Adaptee.FootNote; }
        }

        public string ToolTip
        {
            get { return Adaptee.ToolTip; }
        }

        public int SortIndex
        {
            get { return Adaptee.SortIndex; }
        }

        public IEnumerable<ILabel> Labels
        {
            get { return Adaptee.Labels.Cast<Legacy.Label>().Select(x => new Label(x)); }
        }
    }
}
