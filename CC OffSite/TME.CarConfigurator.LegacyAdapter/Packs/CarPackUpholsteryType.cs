using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.LegacyAdapter.Colours;
using Legacy = TMME.CarConfigurator;


namespace TME.CarConfigurator.LegacyAdapter.Packs
{
    public class CarPackUpholsteryType : CarPackEquipmentItem, ICarPackUpholsteryType
    {
        #region Dependencies (Adaptee)
        private Legacy.CarPackUpholsteryType Adaptee
        {
            get;
            set;
        }
        #endregion

          #region Constructor

        public CarPackUpholsteryType(Legacy.CarPackUpholsteryType adaptee, Legacy.CarUpholsteryType standAloneItemOfTheAdaptee)
            : base(adaptee, standAloneItemOfTheAdaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

        public IReadOnlyList<IColourCombinationInfo> ColourCombinations
        {
            get
            {
                return
                    Adaptee.ColourCombinations.Cast<Legacy.CarColourCombination>()
                        .Select(x => new ColourCombinationInfo(x))
                        .ToList();
            }
        }
    }

}
