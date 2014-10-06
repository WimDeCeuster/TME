using TME.CarConfigurator.Interfaces;

namespace TME.CarConfigurator.QueryRepository
{
    public interface IModelRepository
    {
        IModels GetModels(IContext context);
    }
}

