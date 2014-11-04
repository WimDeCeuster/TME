using System;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Progress;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IMapper
    {
        Task MapAsync(IContext context, IProgress<PublishProgress> progress);
    }
}