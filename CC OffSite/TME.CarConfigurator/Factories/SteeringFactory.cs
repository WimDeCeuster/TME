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
    public class SteeringFactory : ISteeringFactory
    {
        private readonly ISteeringService _steeringService;

        public SteeringFactory(ISteeringService steeringService)
        {
            if (steeringService == null) throw new ArgumentNullException("steeringService");

            _steeringService = steeringService;
        }

        public IEnumerable<ISteering> GetSteerings(Publication publication, Context context)
        {
            return _steeringService.GetSteerings(publication.ID, publication.GetCurrentTimeFrame().ID, context)
                                 .Select(steering => new Steering(steering))
                                 .ToArray();
        }
    }
}
