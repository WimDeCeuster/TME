﻿using System.CodeDom;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.LegacyAdapter.Extensions;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class Grade : BaseObject, IGrade
    {


        #region Dependencies (Adaptee)
        private Legacy.Grade Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public Grade(Legacy.Grade adaptee)
            : base(adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

        public bool Special
        {
            get { return Adaptee.Special; }
        }

        public IPrice StartingPrice
        {
            get { return new StartingPrice(Adaptee); }
        }

        public IGrade BasedUpon
        {
            get
            {
                return Adaptee.BasedUpon==null ? null : new Grade(Adaptee.BasedUpon);
            }
        }

        public IEnumerable<IVisibleInModeAndView> VisibleIn
        {
            get { return Adaptee.Assets.GetVisibleInModeAndViews(); }
        }

        public IEnumerable<IAsset> Assets
        {
            get { return Adaptee.Assets.GetPlainAssets(); }
        }
    }
}