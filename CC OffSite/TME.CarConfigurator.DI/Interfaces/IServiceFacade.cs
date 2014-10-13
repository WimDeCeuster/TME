using TME.CarConfigurator.QueryServices;

namespace TME.CarConfigurator.DI.Interfaces
{
    public interface IServiceFacade
    {
        IServiceFacade WithModelService(IModelService modelService);
        IServiceFacade WithPublicationService(IPublicationService publicationService);
        IServiceFacade WithBodyTypeService(IBodyTypeService bodyTypeService);
        IServiceFacade WithEngineService(IEngineService engineService);

        IModelService CreateModelService();
        IPublicationService CreatePublicationService();
        IBodyTypeService CreateBodyTypeService();
        IEngineService CreateEngineService();

    }
}