using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Factories;

namespace TME.CarConfigurator.Assets
{
    public class VisibleInModeAndView : IVisibleInModeAndView
    {

        private readonly Repository.Objects.Assets.VisibleInModeAndView _repositoryVisibleInModeAndView;
        protected readonly Guid ObjectID;
        protected readonly Repository.Objects.Publication RepositoryPublication;
        protected readonly Repository.Objects.Context RepositoryContext;
        protected readonly IAssetFactory AssetFactory;
        protected IEnumerable<IAsset> FetchedAssets;

        public VisibleInModeAndView(
            Guid objectID,
            Repository.Objects.Assets.VisibleInModeAndView repositoryVisibleInModeAndView,
            Repository.Objects.Publication repositoryPublication,
            Repository.Objects.Context repositoryContext, 
            IAssetFactory assetFactory)
        {
            if (repositoryVisibleInModeAndView == null) throw new ArgumentNullException("repositoryVisibleInModeAndView");
            if (repositoryPublication == null) throw new ArgumentNullException("repositoryPublication");
            if (repositoryContext == null) throw new ArgumentNullException("repositoryContext");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");

            _repositoryVisibleInModeAndView = repositoryVisibleInModeAndView;
            ObjectID = objectID;
            RepositoryPublication = repositoryPublication;
            RepositoryContext = repositoryContext;
            AssetFactory = assetFactory;
        }


        public string Mode
        {
            get { return _repositoryVisibleInModeAndView.Mode; }
        }
        public string View
        {
            get { return _repositoryVisibleInModeAndView.View; }
        }

        public virtual IEnumerable<IAsset> Assets
        {
            get { return FetchedAssets = FetchedAssets ?? AssetFactory.GetAssets(RepositoryPublication, ObjectID, RepositoryContext, View, Mode); }
        }
    }
}
