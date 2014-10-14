using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.Mappers.Exceptions;
using TME.CarConfigurator.Repository.Objects.Assets.Enums;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class AssetScopeMapper : IAssetScopeMapper
    {
        public Scope MapAssetScope(Administration.Assets.Enums.AssetScope scope)
        {
            switch (scope)
            {
                case Administration.Assets.Enums.AssetScope.Internal:
                    return Scope.Internal;
                case Administration.Assets.Enums.AssetScope.Public:
                    return Scope.Public;
                default:
                    throw new UnknownAssetScopeException(scope);
            }
        }
    }
}
