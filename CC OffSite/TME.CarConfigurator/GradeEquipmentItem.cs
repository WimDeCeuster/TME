using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator
{
    public class GradeEquipmentItem : BaseObject, IGradeEquipmentItem
    {
        Repository.Objects.Equipment.GradeEquipmentItem _repositoryEquipmentItem;

        public GradeEquipmentItem(Repository.Objects.Equipment.GradeEquipmentItem repoEquipmentItem)
            : base(repoEquipmentItem)
        {
            if (repoEquipmentItem == null) throw new ArgumentNullException("repoEquipmentItem");

            _repositoryEquipmentItem = repoEquipmentItem;
        }

        public int ShortID { get { return _repositoryEquipmentItem.ShortID; } }

        public string InternalName { get { return _repositoryEquipmentItem.InternalName; } }

        public string PartNumber { get { return _repositoryEquipmentItem.PartNumber; } }

        public string Path { get { return _repositoryEquipmentItem.Path; } }

        public bool KeyFeature { get { return _repositoryEquipmentItem.KeyFeature; } }

        public bool GradeFeature { get { return _repositoryEquipmentItem.GradeFeature; } }

        public bool OptionalGradeFeature { get { return _repositoryEquipmentItem.OptionalGradeFeature; } }

        public bool Brochure { get { return _repositoryEquipmentItem.Brochure; } }

        public bool Standard { get { return _repositoryEquipmentItem.Standard; } }

        public bool Optional { get { return _repositoryEquipmentItem.Optional; } }

        public bool NotAvailable { get { return _repositoryEquipmentItem.NotAvailable; } }

        public Interfaces.Enums.Visibility Visibility { get { throw new NotImplementedException(); } }

        public ICategoryInfo Category { get { throw new NotImplementedException(); } }

        public Interfaces.Colours.IExteriorColour ExteriorColour { get { throw new NotImplementedException(); } }

        public IEnumerable<Interfaces.Assets.IAsset> Assets { get { throw new NotImplementedException(); } }

        public IEnumerable<Interfaces.ILink> Links { get { throw new NotImplementedException(); } }

        public IEnumerable<Interfaces.ICarInfo> StandardOn { get { throw new NotImplementedException(); } }

        public IEnumerable<Interfaces.ICarInfo> OptionalOn { get { throw new NotImplementedException(); } }

        public IEnumerable<Interfaces.ICarInfo> NotAvailableOn { get { throw new NotImplementedException(); } }
    }
}
