using System;
using System.Collections.Generic;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator
{
    public abstract class GradeEquipmentItem : BaseObject<Repository.Objects.Equipment.GradeEquipmentItem>, IGradeEquipmentItem
    {
        ExteriorColour _exteriorColour;
        
        public GradeEquipmentItem(Repository.Objects.Equipment.GradeEquipmentItem repositoryEquipmentItem)
            : base(repositoryEquipmentItem)
        { }

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

        public Interfaces.Enums.Visibility Visibility { get { throw new NotImplementedException(); } }

        public IBestVisibleIn BestVisibleIn
        {
            get { throw new NotImplementedException(); }
        }

        public ICategoryInfo Category { get { throw new NotImplementedException(); } }

        public Interfaces.Colours.IExteriorColour ExteriorColour { get { return RepositoryObject.ExteriorColour == null ? null : _exteriorColour = _exteriorColour ?? new ExteriorColour(RepositoryObject.ExteriorColour); } }

        public IEnumerable<Interfaces.Assets.IAsset> Assets { get { throw new NotImplementedException(); } }

        public IEnumerable<Interfaces.ILink> Links { get { throw new NotImplementedException(); } }

        public IEnumerable<Interfaces.ICarInfo> StandardOn { get { throw new NotImplementedException(); } }

        public IEnumerable<Interfaces.ICarInfo> OptionalOn { get { throw new NotImplementedException(); } }

        public IEnumerable<Interfaces.ICarInfo> NotAvailableOn { get { throw new NotImplementedException(); } }
    }
}
