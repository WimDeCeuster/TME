using TME.CarConfigurator.Interfaces.Assets;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class AssetType : IAssetType
    {                                
        #region Dependencies (Adaptee)
        private Legacy.AssetType Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public AssetType(Legacy.AssetType adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion


        public string Code
        {
            get { return Adaptee.Code; }
        }

        public string Name
        {
            get { return Adaptee.Name; }
        }

        public string Mode
        {
            get { return Adaptee.Mode; }
        }

        public string View
        {
            get { return Adaptee.Details.View; }
        }

        public string Side
        {
            get { return Adaptee.Details.Side; }
        }

        public string Type
        {
            get { return Adaptee.Details.Type; }
        }

        public string ExteriorColourCode
        {
            get { return Adaptee.Details.ExteriorColourCode; }
        }

        public string UpholsteryCode
        {
            get { return Adaptee.Details.UpholsteryCode; }
        }

        public string EquipmentCode
        {
            get { return Adaptee.Details.EquipmentCode; }
        }
    }
}
