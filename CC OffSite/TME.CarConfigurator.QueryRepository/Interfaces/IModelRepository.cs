using TME.CarConfigurator.Interfaces;

namespace TME.CarConfigurator.QueryRepository.Interfaces
{
    public interface IModelRepository
    {
        IModels GetModels(IContext context);
    }
}

