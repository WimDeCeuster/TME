using TME.CarConfigurator.QueryServices;

namespace TME.CarConfigurator.DI.Interfaces
{
    public interface IServiceFacade
    {
        IServiceFacade WithModelService(IModelService modelService);
        IServiceFacade WithPublicationService(IPublicationService publicationService);
        IServiceFacade WithAssetService(IAssetService assetService);

        IModelService CreateModelService();
        IPublicationService CreatePublicationService();
        IAssetService CreateAssetService();
    }
}