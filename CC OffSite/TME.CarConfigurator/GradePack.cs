using System.Collections.Generic;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Packs;

namespace TME.CarConfigurator
{
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