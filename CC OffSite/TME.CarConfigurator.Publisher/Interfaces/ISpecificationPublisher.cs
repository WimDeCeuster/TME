using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Interfaces;


namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface ISpecificationsPublisher
    {
        Task PublishCategoriesAsync(IContext context);
        Task PublishCarTechnicalSpecificationsAsync(IContext context);
    }
}
