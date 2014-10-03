using TME.CarConfigurator.Interfaces;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class Link : ILink
    {
                        
        #region Dependencies (Adaptee)
        private Legacy.Link Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public Link(Legacy.Link adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

        public short ID
        {
            get { return Adaptee.ID; }
        }

        public string Name
        {
            get { return Adaptee.Name; }
        }

        public string Label
        {
            get { return Adaptee.Label; }
        }

        public string Url
        {
            get { return Adaptee.Url; }
        }
    }
}
