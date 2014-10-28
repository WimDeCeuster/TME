using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces.Colours;

namespace TME.CarConfigurator
{
    public class ExteriorColour : BaseObject<Repository.Objects.Colours.ExteriorColour>, IExteriorColour
    {
        ColourTransformation _transformation;

        public ExteriorColour(Repository.Objects.Colours.ExteriorColour repoColor)
            : base(repoColor)
        {

        }

        public IColourTransformation Transformation
        {
            get { return _transformation = _transformation ?? new ColourTransformation(RepositoryObject.Transformation); }
        }
    }
}
