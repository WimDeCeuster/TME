using System;

namespace TME.CarConfigurator.Repository.Objects.Colours
{

    public class ColourTransformation
    {
        public string RGB { get; set; }
        public Decimal Hue { get; set; }
        public Decimal Saturation { get; set; }
        public Decimal Brightness { get; set; }
        public Decimal Contrast { get; set; }
        public Decimal Alpha { get; set; }
    }
}
