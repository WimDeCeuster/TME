using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Factories.Interfaces
{
    public interface IModelFactory
    {
        IModels GetModels(Context context);
    }
}