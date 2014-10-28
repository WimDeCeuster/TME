using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces.Assets;
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

        public bool Promoted
        {
            get { throw new NotImplementedException(); }
        }

        public IColourTransformation Transformation
        {
            get { return _transformation = _transformation ?? new ColourTransformation(RepositoryObject.Transformation); }
        }

        public IExteriorColourType Type
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IAsset> Assets
        {
            get { throw new NotImplementedException(); }
        }
    }
}
