using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Repository.Objects.Colours
{
    public class ExteriorColour : BaseObject
    {
        public bool Promoted { get; set; }
        public ColourTransformation Transformation { get; set; }
        public ExteriorColourType Type { get; set; }
    }
}
