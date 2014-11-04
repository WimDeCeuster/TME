using TME.CarConfigurator.Colours;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces.Colours;
using IExteriorColour = TME.CarConfigurator.Interfaces.Equipment.IExteriorColour;


namespace TME.CarConfigurator.Equipment
{
    public class ExteriorColour : BaseObject<Repository.Objects.Equipment.ExteriorColour>, IExteriorColour
    {
        private IColourTransformation _transformation;

        public ExteriorColour(Repository.Objects.Equipment.ExteriorColour exteriorColour)
            : base(exteriorColour)
        {
        }

        public IColourTransformation Transformation
        {
            get { return _transformation = _transformation ?? new ColourTransformation(RepositoryObject.Transformation); }
        }
    }
}
