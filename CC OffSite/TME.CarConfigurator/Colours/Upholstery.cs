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
    public class Upholstery : BaseObject<Repository.Objects.Colours.Upholstery>, IUpholstery
    {
        protected readonly Publication RepositoryPublication;
        protected readonly Context RepositoryContext;
        protected readonly IAssetFactory AssetFactory;

        private UpholsteryType _type;
        private IReadOnlyList<IAsset> _assets;
        private IReadOnlyList<IVisibleInModeAndView> _fetchedVisibleInModeAndViews;

        public Upholstery(Repository.Objects.Colours.Upholstery repositoryUpholstery, Publication publication, Context context, IAssetFactory assetFactory)
            : base(repositoryUpholstery)
        {
            if (publication == null) throw new ArgumentNullException("publication");
            if (context == null) throw new ArgumentNullException("context");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");

            RepositoryPublication = publication;
            RepositoryContext = context;
            AssetFactory = assetFactory;
        }

        public string InteriorColourCode
        {
            get { return RepositoryObject.InteriorColourCode; }
        }

        public string TrimCode
        {
            get { return RepositoryObject.TrimCode; }
        }

        public IUpholsteryType Type
        {
            get { return _type = _type ?? new UpholsteryType(RepositoryObject.Type); }
        }

        public IReadOnlyList<IVisibleInModeAndView> VisibleIn
        {
            get
            {
                return _fetchedVisibleInModeAndViews = _fetchedVisibleInModeAndViews ?? FetchVisibleInModeAndViews();
            }
        }

        public IReadOnlyList<IAsset> Assets
        {
            get { return _assets = _assets ?? FetchAssets(); }
        }

        protected virtual IReadOnlyList<IVisibleInModeAndView> FetchVisibleInModeAndViews()
        {
            return RepositoryObject.VisibleIn.Select(visibleInModeAndView => new VisibleInModeAndView(RepositoryObject.ID, visibleInModeAndView, RepositoryPublication, RepositoryContext, AssetFactory)).ToList();
        }

        protected virtual IReadOnlyList<IAsset> FetchAssets()
        {
            return AssetFactory.GetAssets(RepositoryPublication, RepositoryObject.ID, RepositoryContext);
        }
    }
}
