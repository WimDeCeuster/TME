using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Factories.Interfaces
{
    public interface IPublicationFactory
    {
        Publication Get(Repository.Objects.Model repositoryModel);
    }
}