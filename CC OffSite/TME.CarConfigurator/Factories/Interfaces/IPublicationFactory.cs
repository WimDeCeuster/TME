using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Factories.Interfaces
{
    public interface IPublicationFactory
    {
        Publication GetPublication(Repository.Objects.Model repositoryModel, IContext context);
    }
}