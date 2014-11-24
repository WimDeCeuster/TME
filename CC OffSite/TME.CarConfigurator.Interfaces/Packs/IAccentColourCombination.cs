
namespace TME.CarConfigurator.Interfaces.Packs
{
    public interface IAccentColourCombination
    {
        Equipment.IExteriorColour BodyColour { get; }
        Equipment.IExteriorColour PrimaryAccentColour { get; }
        Equipment.IExteriorColour SecondaryAccentColour { get; }
        bool Default { get; }
    }
}
