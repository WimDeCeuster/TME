using System;

namespace TME.CarConfigurator.Interfaces.Colours
{

    public interface IColourTransformation
    {
        string RGB { get; }
        Decimal Hue { get; }
        Decimal Saturation { get; }
        Decimal Brightness { get; }
        Decimal Contrast { get; }
        Decimal Alpha { get; }
    }
}
