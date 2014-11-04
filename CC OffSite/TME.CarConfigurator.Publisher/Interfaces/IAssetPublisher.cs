using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Interfaces;


namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IAssetPublisher
    {
        Task PublishAssetsAsync(IContext context);
        Task PublishCarAssetsAsync(IContext context);
        Task PublishSubModelAssetsAsync(IContext context);
    }
}