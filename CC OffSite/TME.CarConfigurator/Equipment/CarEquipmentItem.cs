using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Extensions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Enums;
using TME.CarConfigurator.Interfaces.Equipment;
using IExteriorColour = TME.CarConfigurator.Interfaces.Equipment.IExteriorColour;

namespace TME.CarConfigurator.Equipment
{
    public abstract class CarEquipmentItem<T> : BaseObject<T>, ICarEquipmentItem
        where T  : Repository.Objects.Equipment.CarEquipmentItem
    {
        private BestVisibleIn _bestVisibleIn;
        private CategoryInfo _categoryInfo;
        private List<Link> _links;
        private ExteriorColour _exteriorColour;

        protected CarEquipmentItem(T repositoryObject) 
            : base(repositoryObject)
        {
        }

        public int ShortID { get { return RepositoryObject.ShortID; } }

        public string InternalName { get { return RepositoryObject.InternalName; } }

        public string PartNumber { get { return RepositoryObject.PartNumber; } }

        public string Path { get { return RepositoryObject.Path; } }

        public bool KeyFeature { get { return RepositoryObject.KeyFeature; } }

        public bool GradeFeature { get { return RepositoryObject.GradeFeature; } }

        public bool OptionalGradeFeature { get { return RepositoryObject.OptionalGradeFeature; } }

        [Obsolete]
        public bool Brochure { get { return RepositoryObject.Visibility.HasFlag(Repository.Objects.Enums.Visibility.Brochure); } }

        public bool Standard { get { return RepositoryObject.Standard; } }

        public bool Optional { get { return RepositoryObject.Optional; } }


        public Visibility Visibility { get { return RepositoryObject.Visibility.ToVisibility(); } }

        public IBestVisibleIn BestVisibleIn
        {
            get { return _bestVisibleIn = _bestVisibleIn ?? new BestVisibleIn(RepositoryObject.BestVisibleIn); }
        }

        public ICategoryInfo Category { get { return _categoryInfo = _categoryInfo ?? new CategoryInfo(RepositoryObject.Category); } }

        public IExteriorColour ExteriorColour { get { return RepositoryObject.ExteriorColour == null ? null : _exteriorColour = _exteriorColour ?? new ExteriorColour(RepositoryObject.ExteriorColour); } }

        public IReadOnlyList<ILink> Links { get { return _links = _links ?? RepositoryObject.Links.Select(link => new Link(link)).ToList(); } }

        //restjes
        public abstract IPrice Price { get;  }
        public abstract IReadOnlyList<IVisibleInModeAndView> VisibleIn { get; }
        public abstract IReadOnlyList<IAsset> Assets { get; }
        public abstract IReadOnlyList<IExteriorColourInfo> AvailableForExteriorColours { get;  }
        public abstract IReadOnlyList<IUpholsteryInfo> AvailableForUpholsteries { get; }
    }
}