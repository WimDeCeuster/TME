using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Colours;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.Interfaces.Rules;
using TME.CarConfigurator.Repository.Objects;
using RepositoryCarPack = TME.CarConfigurator.Repository.Objects.Packs.CarPack;

namespace TME.CarConfigurator.Packs
{
    public class CarPack : BaseObject<RepositoryCarPack>, ICarPack
    {
        private readonly IAssetFactory _assetFactory;
        private readonly IEquipmentFactory _equipmentFactory;
        private readonly IRuleFactory _ruleFactory;
        private readonly IColourFactory _colourFactory;

        private readonly Repository.Objects.Publication _publication;
        private readonly Guid _carId;
        private readonly Repository.Objects.Context _context;

        private IPrice _price;
        private IReadOnlyList<IExteriorColourInfo> _availableForExteriorColours;
        private IReadOnlyList<IUpholsteryInfo> _availableForUpholsteries;
        private IReadOnlyList<IAsset> _assets;
        private ICarPackEquipment _equipment;
        private IRuleSets _rules;
        private IReadOnlyList<IAccentColourCombination> _accentColourCombinations;

        public CarPack(RepositoryCarPack pack, Publication publication, Guid carId, Context context, IAssetFactory assetFactory, IEquipmentFactory equipmentFactory, IRuleFactory ruleFactory, IColourFactory colourFactory)
            : base(pack)
        {
            if (publication == null) throw new ArgumentNullException("publication");
            if (context == null) throw new ArgumentNullException("context");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");
            if (equipmentFactory == null) throw new ArgumentNullException("equipmentFactory");
            if (ruleFactory == null) throw new ArgumentNullException("ruleFactory");
            if (colourFactory == null) throw new ArgumentNullException("colourFactory");

            _publication = publication;
            _context = context;
            _assetFactory = assetFactory;
            _equipmentFactory = equipmentFactory;
            _ruleFactory = ruleFactory;
            _colourFactory = colourFactory;
            _carId = carId;
        }

        public bool Standard
        {
            get { return RepositoryObject.Standard; }
        }

        public bool Optional
        {
            get { return RepositoryObject.Optional; }
        }

        public IPrice Price
        {
            get { return _price = _price ?? new Price(RepositoryObject.Price); }
        }

        public int ShortID
        {
            get { return RepositoryObject.ShortID; }
        }

        public bool GradeFeature
        {
            get { return RepositoryObject.GradeFeature; }
        }

        public bool OptionalGradeFeature
        {
            get { return RepositoryObject.OptionalGradeFeature; }
        }

        public IReadOnlyList<IAsset> Assets
        {
            get
            {
                return
                    _assets = _assets ?? _assetFactory.GetCarAssets(_publication, _carId, RepositoryObject.ID, _context);
            }
        }

        public IReadOnlyList<IExteriorColourInfo> AvailableForExteriorColours
        {
            get
            {
                return
                    _availableForExteriorColours =
                        _availableForExteriorColours ??
                        RepositoryObject.AvailableForExteriorColours.Select(info => new ExteriorColourInfo(info))
                            .ToList();
            }
        }

        public IReadOnlyList<IUpholsteryInfo> AvailableForUpholsteries
        {
            get
            {
                return
                    _availableForUpholsteries =
                        _availableForUpholsteries ??
                        RepositoryObject.AvailableForUpholsteries.Select(info => new UpholsteryInfo(info)).ToList();
            }
        }

        public IReadOnlyList<IAccentColourCombination> AccentColourCombinations
        {
            get
            {
                return
                    _accentColourCombinations =
                        _accentColourCombinations ?? _colourFactory.GetCarPackAccentColourCombinations(_publication, _context, _carId, RepositoryObject.ID);
            }
        }

        public ICarPackEquipment Equipment
        {
            get
            {
                return
                    _equipment =
                        _equipment ??
                        _equipmentFactory.GetCarPackEquipment(RepositoryObject.Equipment, _publication, _context, _carId);
            }
        }


        public IRuleSets Rules { get { return _rules = _rules ?? _ruleFactory.GetCarPackRuleSets(RepositoryObject.ID, _carId, _publication, _context); } }
    }
}