using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Interfaces;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IMapper
    {
        Task MapAsync(IContext context);
    }
}