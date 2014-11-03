using TME.CarConfigurator.Interfaces.Factories;

namespace TME.CarConfigurator.DI.Interfaces
{
    public interface IModelFactoryFacade
    {
        IModelFactoryFacade WithServiceFacade(IServiceFacade serviceFacade);
        IModelFactoryFacade WithPublicationFactory(IPublicationFactory publicationFactory);
        IModelFactoryFacade WithBodyTypeFactory(IBodyTypeFactory bodyTypeFactory);
        IModelFactoryFacade WithEngineFactory(IEngineFactory engineFactory);
        IModelFactoryFacade WithTransmissionFactory(ITransmissionFactory transmissionFactory);
        IModelFactoryFacade WithWheelDriveFactory(IWheelDriveFactory wheelDriveFactory);
        IModelFactoryFacade WithSteeringFactory(ISteeringFactory steeringFactory);
        IModelFactoryFacade WithGradeFactory(IGradeFactory gradeFactory);
        IModelFactoryFacade WithCarFactory(ICarFactory carFactory);
        IModelFactoryFacade WithEquipmentFactory(IEquipmentFactory equipmentFactory);
        IModelFactoryFacade WithSpecificationsFactory(ISpecificationsFactory specificationsFactory);
        IModelFactoryFacade WithColourFactory(IColourFactory colourFactory);
        IModelFactoryFacade WithPackFactory(IPackFactory packFactory);

        IModelFactory Create();
    }
}