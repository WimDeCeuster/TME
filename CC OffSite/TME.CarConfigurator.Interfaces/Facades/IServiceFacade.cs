using TME.CarConfigurator.QueryServices;

namespace TME.CarConfigurator.Interfaces.Facades
{
    public interface IServiceFacade
    {
        IServiceFacade WithModelService(IModelService modelService);

        IModelService CreateModelService();
        IPublicationService CreatePublicationService();
    }
}