using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface IWheelDriveFactory
    {
        IReadOnlyList<IWheelDrive> GetWheelDrives(Publication publication, Context context);
        IWheelDrive GetCarWheelDrive(WheelDrive repoWheelDrive, Guid carID, Publication publication, Context context);
    }
}
