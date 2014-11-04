using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Extensions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.Equipment
{
    public abstract class GradeEquipmentItem<T> : BaseObject<T>, IGradeEquipmentItem
        where T : Repository.Objects.Equipment.GradeEquipmentItem
    {
        IExteriorColour _exteriorColour;
        ICategoryInfo _categoryInfo;
        IReadOnlyList<ILink> _links;
        IReadOnlyList<ICarInfo> _standardOn;
        IReadOnlyList<ICarInfo> _optionalOn;
        IReadOnlyList<ICarInfo> _notAvailableOn;
        BestVisibleIn _bestVisibleIn;

        protected GradeEquipmentItem(T repositoryEquipmentItem)
            : base(repositoryEquipmentItem)
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

        public bool NotAvailable { get { return RepositoryObject.NotAvailable; } }

        public Interfaces.Enums.Visibility Visibility { get { return RepositoryObject.Visibility.ToVisibility(); } }

        public IBestVisibleIn BestVisibleIn
        {
            get { return _bestVisibleIn = _bestVisibleIn ?? new BestVisibleIn(RepositoryObject.BestVisibleIn); }
        }

        public ICategoryInfo Category { get { return _categoryInfo = _categoryInfo ?? new CategoryInfo(RepositoryObject.Category); } }

        public IExteriorColour ExteriorColour { get { return RepositoryObject.ExteriorColour == null ? null : _exteriorColour = _exteriorColour ?? new ExteriorColour(RepositoryObject.ExteriorColour); } }

        public IEnumerable<ILink> Links { get { return _links = _links ?? RepositoryObject.Links.Select(link => new Link(link)).ToList(); } }

        public IReadOnlyList<ICarInfo> StandardOn { get { return _standardOn = _standardOn ?? RepositoryObject.StandardOn.Select(carInfo => new CarInfo(carInfo)).ToList(); } }

        public IReadOnlyList<ICarInfo> OptionalOn { get { return _optionalOn = _optionalOn ?? RepositoryObject.OptionalOn.Select(carInfo => new CarInfo(carInfo)).ToList(); } }

        public IReadOnlyList<ICarInfo> NotAvailableOn { get { return _notAvailableOn = _notAvailableOn ?? RepositoryObject.NotAvailableOn.Select(carInfo => new CarInfo(carInfo)).ToList(); } }
    }
}
