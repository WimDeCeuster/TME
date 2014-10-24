using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.Interfaces.Equipment
{
    public interface IGradeEquipment
    {
        IReadOnlyList<IGradeAccessory> GradeAccessories { get; }
        IReadOnlyList<IGradeOption> GradeOptions { get; }
    }
}
