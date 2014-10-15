using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects.Assets.Enums;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IAssetScopeMapper
    {
        Scope MapAssetScope(Administration.Assets.Enums.AssetScope scope);
    }
}
