using System.Collections.Generic;

namespace TME.CarConfigurator.Interfaces.Equipment
{
    public interface ICarEquipment
    {
        IReadOnlyList<ICarAccessory> Accessories { get; }
        IReadOnlyList<ICarOption> Options { get; }
    }
}
