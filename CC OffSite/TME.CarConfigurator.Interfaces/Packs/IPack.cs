using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator.Interfaces.Packs
{
    public interface IPack : IBaseObject
    {
        int ShortID { get; }
        bool GradeFeature { get; }
        bool OptionalGradeFeature { get; }

    }
}
