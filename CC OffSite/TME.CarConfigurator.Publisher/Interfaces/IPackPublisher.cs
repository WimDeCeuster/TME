using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Interfaces;


namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IPackPublisher
    {
        Task PublishGradePacksAsync(IContext context);
        Task PublishSubModelGradePacksAsync(IContext context);
        Task PublishCarPacksAsync(IContext context);
    }
}