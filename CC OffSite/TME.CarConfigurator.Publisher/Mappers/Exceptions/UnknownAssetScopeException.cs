using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.Publisher.Mappers.Exceptions
{
    public class UnknownAssetScopeException : Exception
    {
        public UnknownAssetScopeException(Administration.Assets.Enums.AssetScope scope)
            : base(String.Format("Cannot map unrecognised asset scope value {0}", scope.ToString("g")))
        { }
    }
}
