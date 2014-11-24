using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IWheelDriveMapper
    {
        WheelDrive MapWheelDrive(Administration.ModelGenerationWheelDrive wheelDrive, bool canHaveAssets);
    }
}
