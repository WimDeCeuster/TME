using System.Collections.Generic;

namespace TME.CarConfigurator.Interfaces.Equipment
{
    public interface IGradeEquipment
    {
// ReSharper disable ReturnTypeCanBeEnumerable.Global
        IReadOnlyList<IGradeAccessory> Accessories { get; }
        IReadOnlyList<IGradeOption> Options { get; }
        // ReSharper restore ReturnTypeCanBeEnumerable.Global
    }
}
