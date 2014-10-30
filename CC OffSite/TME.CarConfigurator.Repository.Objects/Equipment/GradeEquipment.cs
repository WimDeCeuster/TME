using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TME.CarConfigurator.Repository.Objects.Equipment
{
    public class GradeEquipment
    {
        public IReadOnlyList<GradeAccessory> Accessories { get; set; }
        public IReadOnlyList<GradeOption> Options { get; set; }
    }
}
