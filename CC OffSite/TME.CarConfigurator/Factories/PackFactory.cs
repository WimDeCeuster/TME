using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Extensions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
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

        public IReadOnlyList<IGradePack> GetGradePacks(Publication publication, Context context, Repository.Objects.Grade grade)
        {
            return _packService.GetGradePacks(publication.ID, publication.GetCurrentTimeFrame().ID, grade.ID, context)
                .Select(repoPack => new GradePack(repoPack))
                .ToList();
        }
    }

    public class GradePack : BaseObject<Repository.Objects.Packs.GradePack>, IGradePack
    {
        public GradePack(Repository.Objects.Packs.GradePack repositoryObject)
            : base(repositoryObject)
        {
        }

        public int ShortID { get; private set; }
        public bool GradeFeature { get; private set; }
        public bool OptionalGradeFeature { get; private set; }
        public IEnumerable<IAsset> Assets { get; private set; }
        public bool Standard { get; private set; }
        public bool Optional { get; private set; }
        public bool NotAvailable { get; private set; }
        public IEnumerable<ICarInfo> StandardOn { get; private set; }
        public IEnumerable<ICarInfo> OptionalOn { get; private set; }
        public IEnumerable<ICarInfo> NotAvailableOn { get; private set; }
    }
}