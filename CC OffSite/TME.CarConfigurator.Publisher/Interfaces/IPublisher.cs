using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Enums.Result;
namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IPublisher
    {
        Task<Result> Publish(IContext context);
    }
}