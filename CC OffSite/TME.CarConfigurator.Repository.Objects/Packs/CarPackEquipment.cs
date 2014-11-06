using System.Collections.Generic;

namespace TME.CarConfigurator.Repository.Objects.Packs
{
    public class CarPackEquipment
    {
        public List<CarPackAccessory> Accessories { get; set; }
        public List<CarPackOption> Options { get; set; }
        public List<CarPackExteriorColourType> ExteriorColourTypes { get; set; }
        public List<CarPackUpholsteryType> UpholsteryTypes { get; set; }
    }
}
