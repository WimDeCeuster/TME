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

        public TransmissionFactory(ITransmissionService transmissionService)
        {
            _transmissionService = transmissionService;
        }

        public IEnumerable<ITransmission> GetTransmissions(Publication publication, Context context)
        {
            return _transmissionService.GetTransmissions(publication.ID, publication.GetCurrentTimeFrame().ID, context)
                                 .Select(transmission => new Transmission(transmission))
                                 .ToArray();
        }
    }
}
