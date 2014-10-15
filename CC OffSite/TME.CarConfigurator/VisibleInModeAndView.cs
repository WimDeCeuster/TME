using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Factories;

namespace TME.CarConfigurator
{
    public class VisibleInModeAndView : IVisibleInModeAndView
    {

        private readonly Guid _objectID;
        private readonly Repository.Objects.Assets.VisibleInModeAndView _repositoryVisibleInModeAndView;
        private readonly Repository.Objects.Publication _repositoryPublication;
        private readonly Repository.Objects.Context _repositoryContext;
        private readonly IAssetFactory _assetFactory;
        private IEnumerable<IAsset> _assets;

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

            _objectID = objectID;
            _repositoryVisibleInModeAndView = repositoryVisibleInModeAndView;
            _repositoryPublication = repositoryPublication;
            _repositoryContext = repositoryContext;
            _assetFactory = assetFactory;
        }


        public string Mode
        {
            get { return _repositoryVisibleInModeAndView.Mode; }
        }
        public string View
        {
            get { return _repositoryVisibleInModeAndView.View; }
        }

        public IEnumerable<IAsset> Assets
        {
            get { return _assets = _assets ?? _assetFactory.GetAssets(_repositoryPublication, _objectID, _repositoryContext, View, Mode); }
        }
    }
}
