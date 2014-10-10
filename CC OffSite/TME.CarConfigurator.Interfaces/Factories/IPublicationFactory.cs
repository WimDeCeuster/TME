using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface IPublicationFactory
    {
        Publication GetPublication(Model repositoryModel, Context context);
    }
}