using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.CommandServices
{
    public interface IWheelDriveService
    {
        Task PutTimeFrameGenerationWheelDrives(String brand, String country, Guid publicationID, Guid timeFrameID, IEnumerable<WheelDrive> wheelDrives);
    }
}
