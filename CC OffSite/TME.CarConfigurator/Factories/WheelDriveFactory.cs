using System;
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
        private readonly IAssetFactory _assetFactory;

        public WheelDriveFactory(IWheelDriveService wheelDriveService, IAssetFactory assetFactory)
        {
            if (wheelDriveService == null) throw new ArgumentNullException("wheelDriveService");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");

            _wheelDriveService = wheelDriveService;
            _assetFactory = assetFactory;
        }

        public IReadOnlyList<IWheelDrive> GetWheelDrives(Publication publication, Context context)
        {
            return _wheelDriveService.GetWheelDrives(publication.ID, publication.GetCurrentTimeFrame().ID, context)
                                 .Select(wheelDrive => new WheelDrive(wheelDrive, publication, context, _assetFactory))
                                 .ToArray();
        }
    }
}
