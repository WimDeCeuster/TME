using TME.CarConfigurator.QueryServices;

namespace TME.CarConfigurator.DI.Interfaces
{
    public interface IServiceFacade
    {
        IServiceFacade WithModelService(IModelService modelService);

        IModelService CreateModelService();
        IPublicationService CreatePublicationService();
    }
}