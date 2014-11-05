using System.Collections.Generic;

namespace TME.CarConfigurator.Interfaces.Equipment
{
    public interface ICarEquipment
    {
// ReSharper disable ReturnTypeCanBeEnumerable.Global
        IReadOnlyList<ICarAccessory> Accessories { get; }
        IReadOnlyList<ICarOption> Options { get; }
        // ReSharper restore ReturnTypeCanBeEnumerable.Global
    }
}
