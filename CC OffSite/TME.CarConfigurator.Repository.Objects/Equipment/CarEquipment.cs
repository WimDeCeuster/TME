using System.Collections.Generic;

namespace TME.CarConfigurator.Repository.Objects.Equipment
{
    public class CarEquipment
    {
        public IReadOnlyList<CarAccessory> Accessories { get; set; }
        public IReadOnlyList<CarOption> Options { get; set; }
    }
}
