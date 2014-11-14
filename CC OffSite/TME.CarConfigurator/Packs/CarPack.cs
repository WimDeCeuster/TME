using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Packs;
using RepositoryCarPack = TME.CarConfigurator.Repository.Objects.Packs.CarPack;

namespace TME.CarConfigurator.Packs
{
    public class CarPack : BaseObject<RepositoryCarPack>, ICarPack
    {
        private IPrice _price;
        
        public CarPack(RepositoryCarPack pack)
            : base(pack)
        {

        }

        public bool Standard { get { return RepositoryObject.Standard; } }

        public bool Optional { get { return RepositoryObject.Optional; } }

        public IPrice Price { get { return _price = _price ?? new Price(RepositoryObject.Price); } }

        public int ShortID { get { return RepositoryObject.ShortID; } }

        public bool GradeFeature { get { return RepositoryObject.GradeFeature; } }

        public bool OptionalGradeFeature { get { return RepositoryObject.OptionalGradeFeature; } }

        public IReadOnlyList<IAsset> Assets { get { throw new NotImplementedException(); } }

        public IReadOnlyList<IExteriorColourInfo> AvailableForExteriorColours { get { return new List<IExteriorColourInfo>(); } }

        public IReadOnlyList<IUpholsteryInfo> AvailableForUpholsteries { get { return new List<IUpholsteryInfo>(); } }

        public ICarPackEquipment Equipment { get { throw new NotImplementedException(); } }
    }
}
