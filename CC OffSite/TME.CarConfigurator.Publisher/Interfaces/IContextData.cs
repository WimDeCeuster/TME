using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IContextData
    {
        IRepository<Car> Cars { get; }
        IRepository<Generation> Generations { get; }
    }
}