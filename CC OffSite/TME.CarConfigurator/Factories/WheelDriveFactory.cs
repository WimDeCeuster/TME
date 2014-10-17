using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Extensions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Factories
{
    public class WheelDriveFactory : IWheelDriveFactory
    {
        private readonly IWheelDriveService _wheelDriveService;

        public WheelDriveFactory(IWheelDriveService wheelDriveService)
        {
            _wheelDriveService = wheelDriveService;
        }

        public IEnumerable<IWheelDrive> GetWheelDrives(Publication publication, Context context)
        {
            return _wheelDriveService.GetWheelDrives(publication.ID, publication.GetCurrentTimeFrame().ID, context)
                                 .Select(wheelDrive => new WheelDrive(wheelDrive))
                                 .ToArray();
        }
    }
}
