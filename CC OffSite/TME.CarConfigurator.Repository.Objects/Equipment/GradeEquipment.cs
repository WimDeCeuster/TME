using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TME.CarConfigurator.Repository.Objects.Equipment
{
    public class GradeEquipment
    {
        public IEnumerable<GradeAccessory> Accessories { get; set; }
        public IEnumerable<GradeOption> Options { get; set; }
    }
}
