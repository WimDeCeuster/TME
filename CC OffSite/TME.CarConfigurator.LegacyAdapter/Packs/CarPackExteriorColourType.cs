using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.LegacyAdapter.Colours;
using Legacy = TMME.CarConfigurator;


namespace TME.CarConfigurator.LegacyAdapter.Packs
{
    public class CarPackExteriorColourType : CarPackEquipmentItem, ICarPackExteriorColourType
    {
        #region Dependencies (Adaptee)
        private Legacy.CarPackExteriorColourType Adaptee
        {
            get;
            set;
        }
        #endregion

          #region Constructor

        public CarPackExteriorColourType(Legacy.CarPackExteriorColourType adaptee, Legacy.CarExteriorColourType standAloneItemOfTheAdaptee)
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
