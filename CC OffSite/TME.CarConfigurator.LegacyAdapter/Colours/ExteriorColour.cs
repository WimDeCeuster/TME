using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator.LegacyAdapter.Colours
{
    public class ExteriorColour :  IExteriorColour
    {
        #region Dependencies (Adaptee)
        private TMME.CarConfigurator.ExteriorColour Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public ExteriorColour(TMME.CarConfigurator.ExteriorColour adaptee) 
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
            get { return Adaptee.Code; }
        }

        public string LocalCode
        {
            get { return string.Empty; }
        }

        public string Name
        {
            get
            {
                try
                {
                    return Adaptee.Name;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
    
            }
        }

        public string Description
        {
            get { return string.Empty; }
        }

        public string FootNote
        {
            get { return string.Empty; }
        }

        public string ToolTip
        {
            get { return string.Empty; }
        }

        public int SortIndex
        {
            get { return 0; }
        }


        public bool Promoted
        {
            get { return false; }
        }

        public IColourTransformation Transformation
        {
            get
            {

                try
                {
                    return new ColourTransformation(Adaptee.Transformation);
                }
                catch (Exception)
                {
                    return null;
                }

               
            }
        }

        public IExteriorColourType Type
        {
            get { return null; }
        }

        public IReadOnlyList<IAsset> Assets
        {
            get { return new List<IAsset>(); }
        }


        public IEnumerable<ILabel> Labels
        {
            get
            {

                try
                {
                    return Adaptee.Labels.Cast<TMME.CarConfigurator.Label>().Select(x => new Label(x)).ToList(); 
                }
                catch (Exception)
                {
                    return new List<ILabel>();
                }
            }
        }
    }
}
