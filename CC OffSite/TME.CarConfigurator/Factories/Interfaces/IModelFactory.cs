using TME.CarConfigurator.Interfaces;

namespace TME.CarConfigurator.Factories.Interfaces
{
    public interface IModelFactory
    {
        IModels Get(IContext context);
    }
}