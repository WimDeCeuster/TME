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
    public class TransmissionFactory : ITransmissionFactory
    {
        private readonly ITransmissionService _transmissionService;
        private readonly IAssetFactory _assetFactory;

        public TransmissionFactory(ITransmissionService transmissionService, IAssetFactory assetFactory)
        {
            if (transmissionService == null) throw new ArgumentNullException("transmissionService");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");

            _transmissionService = transmissionService;
            _assetFactory = assetFactory;
        }

        public IReadOnlyList<ITransmission> GetTransmissions(Publication publication, Context context)
        {
            return _transmissionService.GetTransmissions(publication.ID, publication.GetCurrentTimeFrame().ID, context)
                                 .Select(transmission => new Transmission(transmission,publication,context,_assetFactory))
                                 .ToArray();
        }

        public ITransmission GetCarTransmission(Repository.Objects.Transmission transmission, Guid carID, Publication repositoryPublication,
            Context repositoryContext)
        {
            return new CarTransmission(transmission, repositoryPublication, carID, repositoryContext, _assetFactory);
        }
    }
}
