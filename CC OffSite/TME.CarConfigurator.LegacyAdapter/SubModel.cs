using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.LegacyAdapter.Equipment;
using TME.CarConfigurator.LegacyAdapter.Extensions;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class SubModel : BaseObject, ISubModel
    {
        #region Dependencies (Adaptee)
        private Legacy.SubModel Adaptee { get; set; }
        #endregion

        #region Constructor
        public SubModel(Legacy.SubModel adaptee) 
            : base(adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion


        private IEnumerable<Legacy.Car> GetCars()
        {
            return Adaptee.Generation.Cars
                .Cast<Legacy.Car>()
                .Where(x => x.SubModel.ID == Adaptee.ID);
        }

        public IPrice StartingPrice
        {
            get
            {
                return new StartingPrice(
                    GetCars()
                        .OrderBy(x => x.MinimumPriceInVat)
                        .First()
                        );
            }
        }
        
        public IEnumerable<IEquipmentItem> Equipment
        {
            get
            {
                return
                    GetCars()
                    .SelectMany(car => car.Equipment.Cast<Legacy.CarEquipmentItem>())
                    .Distinct()
                    .Select(x => 
                        (   x.Type == Legacy.EquipmentType.Accessory
                                ? (IEquipmentItem)new Accessory((Legacy.CarAccessory)x, Adaptee.Generation)
                                : (IEquipmentItem)new Option((Legacy.CarOption)x, Adaptee.Generation)
                        )
                    );

            }
        }

        public IEnumerable<IGrade> Grades
        {
            get { return Adaptee.Grades.Cast<Legacy.Grade>().Select(x => new Grade(x)); }
        }

        public IReadOnlyList<IAsset> Assets
        {
            get { return Adaptee.Assets.GetPlainAssets(); }
        }

        public IEnumerable<ILink> Links
        {
            get { return Adaptee.Links.Cast<Legacy.Link>().Select(x => new Link(x)); }
        }
    }
}