using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Colours
{
    public class ColourCombination : IColourCombination
    {
        protected readonly Repository.Objects.Colours.ColourCombination RepositoryColourCombination;
        protected readonly IColourFactory ColourFactory;
        protected readonly IAssetFactory AssetFactory;
        protected readonly Context RepositoryContext;
        protected readonly Publication RepositoryPublication;
        
        private IExteriorColour _exteriorColour;
        private IUpholstery _upholstery;

        public ColourCombination(Repository.Objects.Colours.ColourCombination repositoryColourCombination, Publication repositoryPublication, Context repositoryContext, IColourFactory colourFactory, IAssetFactory assetFactory)
        {
            if (repositoryColourCombination == null) throw new ArgumentNullException("repositoryColourCombination");
            if (repositoryPublication == null) throw new ArgumentNullException("repositoryPublication");
            if (repositoryContext == null) throw new ArgumentNullException("repositoryContext");
            if (colourFactory == null) throw new ArgumentNullException("colourFactory");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");

            RepositoryColourCombination = repositoryColourCombination;
            RepositoryPublication = repositoryPublication;
            RepositoryContext = repositoryContext;
            ColourFactory = colourFactory;
            AssetFactory = assetFactory;
        }
    
        public Guid ID
        {
	        get { return RepositoryColourCombination.ID; }
        }

        public IExteriorColour ExteriorColour
        {
            get { return _exteriorColour = _exteriorColour ?? ColourFactory.GetExteriorColour(RepositoryColourCombination.ExteriorColour, RepositoryPublication, RepositoryContext); }
        }

        public IUpholstery Upholstery
        {
            get { return _upholstery = _upholstery ?? ColourFactory.GetUpholstery(RepositoryColourCombination.Upholstery, RepositoryPublication, RepositoryContext); }
        }

        public int SortIndex
        {
            get { return RepositoryColourCombination.SortIndex; }
        }

        public IReadOnlyList<IVisibleInModeAndView> VisibleIn
        {
            get
            {
                //TODO: provide implementation
                return new List<IVisibleInModeAndView>();
            }
        }

        public IReadOnlyList<IAsset> Assets
        {
            get
            {
                //TODO: provide implementation
                return new List<IAsset>();
            }
        }
    }
}
