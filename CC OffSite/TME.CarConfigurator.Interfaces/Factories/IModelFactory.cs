using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface IModelFactory
    {
        IModels GetModels(Context context);
    }
}