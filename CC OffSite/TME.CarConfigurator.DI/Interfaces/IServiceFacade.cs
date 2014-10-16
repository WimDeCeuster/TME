using TME.CarConfigurator.QueryServices;

namespace TME.CarConfigurator.DI.Interfaces
{
    public interface IServiceFacade
    {
        IServiceFacade WithModelService(IModelService modelService);
        IServiceFacade WithPublicationService(IPublicationService publicationService);
        IServiceFacade WithBodyTypeService(IBodyTypeService bodyTypeService);
        IServiceFacade WithEngineService(IEngineService engineService);
        IServiceFacade WithTransmissionService(ITransmissionService transmissionService);
        IServiceFacade WithCarService(ICarService carService);

        IModelService CreateModelService();
        IPublicationService CreatePublicationService();
        IBodyTypeService CreateBodyTypeService();
        IAssetService CreateAssetService();
        IEngineService CreateEngineService();
        ITransmissionService CreateTransmissionService();
        ICarService CreateCarService();

    }
}