using System.Threading.Tasks;

namespace TME.CarConfigurator.Publisher.Job
{
    internal interface IJob
    {
        Task Run();
    }
}