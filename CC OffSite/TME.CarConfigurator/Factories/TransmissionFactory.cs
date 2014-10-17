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

        public IEnumerable<ITransmission> GetTransmissions(Publication publication, Context context)
        {

            var repoTransmission = _transmissionService.GetTransmissions(publication.ID,
                publication.GetCurrentTimeFrame().ID, context);

            return repoTransmission.Select(tran => new Transmission(tran, publication, context, _assetFactory)).ToArray();
            return _transmissionService.GetTransmissions(publication.ID, publication.GetCurrentTimeFrame().ID, context)
                                 .Select(transmission => new Transmission(transmission,publication,context,_assetFactory))
                                 .ToArray();
        }
    }
}
