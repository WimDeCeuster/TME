using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Extensions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator
{
    public class SubModel : BaseObject, ISubModel
    {
        private readonly Repository.Objects.SubModel _repositorySubModel;
        private readonly Publication _repositoryPublication;
        private readonly Context _repositoryContext;
        private readonly IAssetFactory _assetFactory;

        private IEnumerable<IAsset> _assets;
        private IEnumerable<IVisibleInModeAndView> _visibleInModeAndViews;

        public SubModel(Repository.Objects.SubModel repositorySubModel,Publication repositoryPublication,Context repositoryContext,IAssetFactory assetFactory) 
            : base(repositorySubModel)
        {
            if (repositorySubModel == null) throw new ArgumentNullException("repositorySubModel");
            if (repositoryPublication == null) throw new ArgumentNullException("repositoryPublication");
            if (repositoryContext == null) throw new ArgumentNullException("repositoryContext");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");

            _repositorySubModel = repositorySubModel;
            _repositoryPublication = repositoryPublication;
            _repositoryContext = repositoryContext;
            _assetFactory = assetFactory;
        }

        public IGeneration Generation { get; set; }

        public IEnumerable<IVisibleInModeAndView> VisibleIn
        {
            get
            {
                return
                    _visibleInModeAndViews =
                        _visibleInModeAndViews ??
                        _repositorySubModel.VisibleIn.Select(
                            visibleInModeAndView =>
                                new VisibleInModeAndView(_repositorySubModel.ID, visibleInModeAndView,
                                    _repositoryPublication, _repositoryContext, _assetFactory)).ToList();
            }
        }

        public IEnumerable<IAsset> Assets { get { return _assets = _assets ?? _assetFactory.GetAssets(_repositoryPublication, ID, _repositoryContext); } }

        [Obsolete("Use the new VisibleIn property instead")]
        public bool VisibleInExteriorSpin { get { return VisibleIn.VisibleInExteriorSpin(); } }
        [Obsolete("Use the new VisibleIn property instead")]
        public bool VisibleInInteriorSpin { get { return VisibleIn.VisibleInInteriorSpin(); } }
        [Obsolete("Use the new VisibleIn property instead")]
        public bool VisibleInXRay4X4Spin { get { return VisibleIn.VisibleInXRay4X4Spin(); } }
        [Obsolete("Use the new VisibleIn property instead")]
        public bool VisibleInXRayHybridSpin { get { return VisibleIn.VisibleInXRayHybridSpin(); } }
        [Obsolete("Use the new VisibleIn property instead")]
        public bool VisibleInXRaySafetySpin { get { return VisibleIn.VisibleInXRaySafetySpin(); } }
    }
}