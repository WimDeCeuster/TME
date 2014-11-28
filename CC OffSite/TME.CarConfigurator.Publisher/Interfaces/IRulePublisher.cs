using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Interfaces;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IRulePublisher
    {
        Task PublishCarRulesAsync(IContext context);
    }
}