using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Colours;
using TME.CarConfigurator.Extensions;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Packs;

namespace TME.CarConfigurator.Factories
{
    public class ColourFactory : IColourFactory
    {
        private readonly IColourService _colourService;
        private readonly IAssetFactory _assetFactory;
        private readonly Dictionary<Guid, IDictionary<Guid,IEnumerable<AccentColourCombination>>> _carPackAccentColourCombination = new Dictionary<Guid, IDictionary<Guid, IEnumerable<AccentColourCombination>>>();

        public ColourFactory(IColourService colourService, IAssetFactory assetFactory)
        {
            if (colourService == null) throw new ArgumentNullException("colourService");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");
            
            _colourService = colourService;
            _assetFactory = assetFactory;
        }

        public IReadOnlyList<IColourCombination> GetColourCombinations(Publication publication, Context context)
        {
            return _colourService.GetColourCombinations(publication.ID, publication.GetCurrentTimeFrame().ID, context)
                                  .Select(colour => new ColourCombination(colour, publication, context, this, _assetFactory))
                                  .ToList();
        }

        public IReadOnlyList<ICarColourCombination> GetCarColourCombinations(Publication publication, Context context, Guid carID)
        {
            return _colourService.GetCarColourCombinations(publication.ID, context, carID)
                .Select(colour => new CarColourCombination(carID, colour, publication, context, _assetFactory))
                .ToList();
        }

        public IUpholstery GetUpholstery(Repository.Objects.Colours.Upholstery repositoryUpholstery, Publication publication, Context context)
        {
            return new Upholstery(repositoryUpholstery, publication, context, _assetFactory);
        }

        public IExteriorColour GetExteriorColour(Repository.Objects.Colours.ExteriorColour repositoryExteriorColour, Publication publication, Context context)
        {
            return new ExteriorColour(repositoryExteriorColour, publication, context, _assetFactory);
        }

        public IReadOnlyList<IAccentColourCombination> GetCarPackAccentColourCombinations(Publication publication, Context context, Guid carID, Guid packID)
        {
            if (!_carPackAccentColourCombination.ContainsKey(carID))
                _carPackAccentColourCombination.Add(carID, _colourService.GetCarPackAccentColourCombinations(carID, publication.ID, context));

            return FilterByPack(_carPackAccentColourCombination[carID], packID);
        }

        private IReadOnlyList<IAccentColourCombination> FilterByPack(IDictionary<Guid, IEnumerable<AccentColourCombination>> packAccentColourCombinations, Guid packID)
        {
            if (!packAccentColourCombinations.ContainsKey(packID))
                packAccentColourCombinations.Add(packID, new List<AccentColourCombination>());

            return packAccentColourCombinations[packID].Select(accentColourCombination => new Packs.AccentColourCombination(accentColourCombination)).ToList();
        }
    }
}
