using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Repository.Objects.Packs
{
    public class Pack: BaseObject
    {
        public int ShortID { get; set; }
        public bool GradeFeature { get; set; }
        public bool OptionalGradeFeature { get; set; }
    }
}
