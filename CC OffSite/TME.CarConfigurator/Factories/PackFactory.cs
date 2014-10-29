using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Extensions;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Factories
{
    public class PackFactory : IPackFactory
    {
        private readonly IPackService _packService;

        public PackFactory(IPackService packService)
        {
            if (packService == null) throw new ArgumentNullException("packService");
            _packService = packService;
        }

        public IReadOnlyList<IGradePack> GetGradePacks(Publication publication, Context context, Guid gradeId)
        {
            return _packService.GetGradePacks(publication.ID, publication.GetCurrentTimeFrame().ID, gradeId, context)
                .Select(repoPack => new GradePack(repoPack))
                .ToList();
        }
    }
}