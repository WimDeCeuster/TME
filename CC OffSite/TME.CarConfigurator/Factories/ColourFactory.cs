using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Colours;
using TME.CarConfigurator.Extensions;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Factories
{
    public class ColourFactory : IColourFactory
    {
        private readonly IColourService _colourService;
        private readonly IAssetFactory _assetFactory;

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
                .Select(colour => new CarColourCombination(carID, colour, publication, context, this, _assetFactory))
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
    }
}
