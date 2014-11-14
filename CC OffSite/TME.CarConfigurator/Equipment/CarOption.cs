using System;
using System.Collections.Generic;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.Equipment
{
    public class CarOption : CarEquipmentItem<Repository.Objects.Equipment.CarOption>, ICarOption
    {
        private readonly IOptionInfo _parentOptionInfo;

        public CarOption(Repository.Objects.Equipment.CarOption repositoryObject, IOptionInfo parentOptionInfo) 
            : base(repositoryObject)
        {
            _parentOptionInfo = parentOptionInfo;
        }

        public bool TechnologyItem { get { return RepositoryObject.TechnologyItem; } }

        public IOptionInfo ParentOption { get { return _parentOptionInfo; } }

        public bool PostProductionOption { get { throw new NotImplementedException(); } }
        public bool SuffixOption { get { throw new NotImplementedException(); } }

        public override IPrice Price { get { return new Price(RepositoryObject.Price); } }

        public override IReadOnlyList<IVisibleInModeAndView> VisibleIn
        {
            get { throw new NotImplementedException(); }
        }

        public override IReadOnlyList<IAsset> Assets
        {
            get { throw new NotImplementedException(); }
        }

        public override IReadOnlyList<IExteriorColourInfo> AvailableForExteriorColours
        {
            get { throw new NotImplementedException(); }
        }

        public override IReadOnlyList<IUpholsteryInfo> AvailableForUpholsteries
        {
            get { throw new NotImplementedException(); }
        }
    }
}