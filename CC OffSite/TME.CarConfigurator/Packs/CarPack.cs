using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Colours;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Interfaces.Packs;
using RepositoryCarPack = TME.CarConfigurator.Repository.Objects.Packs.CarPack;

namespace TME.CarConfigurator.Packs
{
    public class CarPack : BaseObject<RepositoryCarPack>, ICarPack
    {
        private readonly IAssetFactory _assetFactory;
        
        private readonly Repository.Objects.Publication _publication;
        private readonly Guid _carId;
        private readonly Repository.Objects.Context _context;
        
        private IPrice _price;
        private IReadOnlyList<IExteriorColourInfo> _availableForExteriorColours;
        private IReadOnlyList<IUpholsteryInfo> _availableForUpholsteries;
        private IReadOnlyList<IAsset> _assets;

        public CarPack(RepositoryCarPack pack, Repository.Objects.Publication publication, Guid carId, Repository.Objects.Context context, IAssetFactory assetFactory)
            : base(pack)
        {
            if (publication == null) throw new ArgumentNullException("repositoryPublication");
            if (context == null) throw new ArgumentNullException("repositoryContext");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");

            _publication = publication;
            _context = context;
            _assetFactory = assetFactory;
            _carId = carId;
        }

        public bool Standard { get { return RepositoryObject.Standard; } }

        public bool Optional { get { return RepositoryObject.Optional; } }

        public IPrice Price { get { return _price = _price ?? new Price(RepositoryObject.Price); } }

        public int ShortID { get { return RepositoryObject.ShortID; } }

        public bool GradeFeature { get { return RepositoryObject.GradeFeature; } }

        public bool OptionalGradeFeature { get { return RepositoryObject.OptionalGradeFeature; } }

        public IReadOnlyList<IAsset> Assets { get { return _assets = _assets ?? _assetFactory.GetCarAssets(_publication, _carId, RepositoryObject.ID, _context); } }

        public IReadOnlyList<IExteriorColourInfo> AvailableForExteriorColours { get { return _availableForExteriorColours = _availableForExteriorColours ?? RepositoryObject.AvailableForExteriorColours.Select(info => new ExteriorColourInfo(info)).ToList(); } }

        public IReadOnlyList<IUpholsteryInfo> AvailableForUpholsteries { get { return _availableForUpholsteries = _availableForUpholsteries ?? RepositoryObject.AvailableForUpholsteries.Select(info => new UpholsteryInfo(info)).ToList(); } }

        public ICarPackEquipment Equipment { get { throw new NotImplementedException(); } }
    }
}