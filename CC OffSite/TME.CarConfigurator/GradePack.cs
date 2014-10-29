using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Packs;

namespace TME.CarConfigurator
{
    public class GradePack : BaseObject<Repository.Objects.Packs.GradePack>, IGradePack
    {
        private IReadOnlyList<CarInfo> _standardOn;
        private IReadOnlyList<CarInfo> _optionalOn;
        private IReadOnlyList<CarInfo> _notAvailableOn;

        public GradePack(Repository.Objects.Packs.GradePack repositoryObject)
            : base(repositoryObject)
        {
        }

        public int ShortID { get { return RepositoryObject.ShortID; } }
        public bool GradeFeature { get { return RepositoryObject.GradeFeature; } }
        public bool OptionalGradeFeature { get { return RepositoryObject.OptionalGradeFeature; } }
        public IEnumerable<IAsset> Assets { get { return new List<IAsset>(); } }
        public bool Standard { get { return RepositoryObject.Standard; } }
        public bool Optional { get { return RepositoryObject.Optional; } }
        public bool NotAvailable { get { return RepositoryObject.NotAvailable; } }
        public IReadOnlyList<ICarInfo> StandardOn { get { return _standardOn = _standardOn ?? RepositoryObject.StandardOn.Select(ci => new CarInfo(ci)).ToList(); } }
        public IReadOnlyList<ICarInfo> OptionalOn { get { return _optionalOn = _optionalOn ?? RepositoryObject.OptionalOn.Select(ci => new CarInfo(ci)).ToList(); } }
        public IReadOnlyList<ICarInfo> NotAvailableOn { get { return _notAvailableOn = _notAvailableOn ?? RepositoryObject.NotAvailableOn.Select(ci => new CarInfo(ci)).ToList(); } }
    }
}