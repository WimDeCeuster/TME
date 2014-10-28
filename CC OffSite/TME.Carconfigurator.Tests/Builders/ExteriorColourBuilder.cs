using TME.CarConfigurator.Repository.Objects.Colours;

namespace TME.Carconfigurator.Tests.Builders
{
    public class ExteriorColourBuilder
    {
        private readonly ExteriorColour _colourTransformation;

        public ExteriorColourBuilder()
        {
            _colourTransformation = new ExteriorColour();
        }

        public ExteriorColour Build()
        {
            return _colourTransformation;
        }
    }
}