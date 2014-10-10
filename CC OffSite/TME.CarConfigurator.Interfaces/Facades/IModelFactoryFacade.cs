using TME.CarConfigurator.Interfaces.Factories;

namespace TME.CarConfigurator.Interfaces.Facades
{
    public interface IModelFactoryFacade
    {
        IModelFactoryFacade WithServiceFacade(IServiceFacade serviceFacade);
        IModelFactoryFacade WithPublicationFactory(IPublicationFactory publicationFactory);

        IModelFactory Create();
    }
}