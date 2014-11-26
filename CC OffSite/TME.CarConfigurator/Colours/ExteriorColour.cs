using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Assets;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Colours
{
    public class ExteriorColour : BaseObject<Repository.Objects.Colours.ExteriorColour>, IExteriorColour
    {
        protected readonly Publication RepositoryPublication;
        protected readonly Context RepositoryContext;
        protected readonly IAssetFactory AssetFactory;

        private ColourTransformation _transformation;
        private ExteriorColourType _type;
        private IReadOnlyList<IAsset> _fetchedAssets;
        private IReadOnlyList<IVisibleInModeAndView> _fetchedVisibleInModeAndViews;

        public ExteriorColour(Repository.Objects.Colours.ExteriorColour repositoryColour, Publication publication, Context context, IAssetFactory assetFactory)
            : base(repositoryColour)
        {
            if (publication == null) throw new ArgumentNullException("publication");
            if (context == null) throw new ArgumentNullException("context");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");

            RepositoryPublication = publication;
            RepositoryContext = context;
            AssetFactory = assetFactory;
        }

        public bool Promoted
        {
            get { return RepositoryObject.Promoted; }
        }

        public IColourTransformation Transformation
        {
            get { return RepositoryObject.Transformation == null ? null : _transformation = _transformation ?? new ColourTransformation(RepositoryObject.Transformation); }
        }

        public IExteriorColourType Type
        {
            get { return _type = _type ?? new ExteriorColourType(RepositoryObject.Type); }
        }

        public IReadOnlyList<IVisibleInModeAndView> VisibleIn
        {
            get
            {
                return _fetchedVisibleInModeAndViews = _fetchedVisibleInModeAndViews ?? GetFetchedVisibleInModeAndViews();
            }
        }

        public IReadOnlyList<IAsset> Assets
        {
            get { return _fetchedAssets = _fetchedAssets ?? FetchAssets(); }
        }

        protected virtual IReadOnlyList<IVisibleInModeAndView> GetFetchedVisibleInModeAndViews()
        {
            return RepositoryObject.VisibleIn.Select(visibleInModeAndView => new VisibleInModeAndView(RepositoryObject.ID, visibleInModeAndView, RepositoryPublication, RepositoryContext, AssetFactory)).ToList();
        }

        protected virtual IReadOnlyList<IAsset> FetchAssets()
        {
            return AssetFactory.GetAssets(RepositoryPublication, RepositoryObject.ID, RepositoryContext);
        }
    }
}
