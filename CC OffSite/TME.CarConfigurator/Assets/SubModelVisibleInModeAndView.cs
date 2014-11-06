using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Assets
{
    public class SubModelVisibleInModeAndView : VisibleInModeAndView
    {
        readonly Guid _subModelID;

        public SubModelVisibleInModeAndView(
            Guid subModelID, 
            Guid objectID, 
            Repository.Objects.Assets.VisibleInModeAndView visibleIn, 
            Publication publication, 
            Context context, 
            IAssetFactory assetFactory)
            : base(objectID,visibleIn,publication,context,assetFactory)
        {
            _subModelID = subModelID;
        }

        protected override IReadOnlyList<IAsset> FetchAssets()
        {
            return AssetFactory.GetSubModelAssets(RepositoryPublication, _subModelID, ObjectID, RepositoryContext, View, Mode);
        }
    }
}