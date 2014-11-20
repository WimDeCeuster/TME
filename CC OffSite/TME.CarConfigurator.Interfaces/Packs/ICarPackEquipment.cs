using System.Collections.Generic;

namespace TME.CarConfigurator.Interfaces.Packs
{
    public interface ICarPackEquipment
    {
        // ReSharper disable ReturnTypeCanBeEnumerable.Global
        IReadOnlyList<ICarPackAccessory> Accessories { get; }
        IReadOnlyList<ICarPackOption> Options { get; }
        IReadOnlyList<ICarPackExteriorColourType> ExteriorColourTypes { get; }
        IReadOnlyList<ICarPackUpholsteryType> UpholsteryTypes { get; }
        // ReSharper restore ReturnTypeCanBeEnumerable.Global
    }
}
