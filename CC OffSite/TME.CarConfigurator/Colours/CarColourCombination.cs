using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Colours
{
    public class CarColourCombination : ICarColourCombination
    {
        private readonly Guid _carID;
        private readonly Repository.Objects.Colours.CarColourCombination _repositoryColourCombination;
        private readonly Publication _repositoryPublication;
        private readonly Context _repositoryContext;
        private readonly IColourFactory _colourFactory;
        private readonly IAssetFactory _assetFactory;
        private CarExteriorColour _exteriorColour;
        private CarUpholstery _upholstery;


        public CarColourCombination(Guid carID, Repository.Objects.Colours.CarColourCombination repositoryColourCombination, Publication repositoryPublication, Context repositoryContext, IColourFactory colourFactory, IAssetFactory assetFactory)
        {
            if (repositoryColourCombination == null) throw new ArgumentNullException("repositoryColourCombination");
            if (repositoryPublication == null) throw new ArgumentNullException("repositoryPublication");
            if (repositoryContext == null) throw new ArgumentNullException("repositoryContext");
            if (colourFactory == null) throw new ArgumentNullException("colourFactory");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");

            _carID = carID;
            _repositoryColourCombination = repositoryColourCombination;
            _repositoryPublication = repositoryPublication;
            _repositoryContext = repositoryContext;
            _colourFactory = colourFactory;
            _assetFactory = assetFactory;
        }

        public int SortIndex { get { return _repositoryColourCombination.SortIndex; } }
        public Guid ID { get { return _repositoryColourCombination.ID; } }

        public ICarExteriorColour ExteriorColour { get { return _exteriorColour = _exteriorColour ?? new CarExteriorColour(_carID, _repositoryColourCombination.ExteriorColour, _repositoryPublication, _repositoryContext, _assetFactory);} }
        public ICarUpholstery Upholstery { get { return _upholstery = _upholstery ?? new CarUpholstery(_carID,_repositoryColourCombination.Upholstery, _repositoryPublication, _repositoryContext, _assetFactory); } }

        public IReadOnlyList<IVisibleInModeAndView> VisibleIn { get { throw new NotImplementedException(); } }
        public IReadOnlyList<IAsset> Assets { get { throw new NotImplementedException(); } }
    }
}