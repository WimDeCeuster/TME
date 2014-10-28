using System.Collections.Generic;

namespace TME.CarConfigurator.Interfaces.Equipment
{
    public interface IGradeEquipment
    {
        IReadOnlyList<IGradeAccessory> GradeAccessories { get; }
        IReadOnlyList<IGradeOption> GradeOptions { get; }
    }
}
