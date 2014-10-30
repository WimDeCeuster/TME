using System.Collections.Generic;

namespace TME.CarConfigurator.Interfaces.Equipment
{
    public interface IGradeEquipment
    {
        IReadOnlyList<IGradeAccessory> Accessories { get; }
        IReadOnlyList<IGradeOption> Options { get; }
    }
}
