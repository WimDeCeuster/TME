using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.FrontEndViewer.Models
{
    public class GradeEquipmentModel
    {
        public IReadOnlyList<IGradeAccessory> Accessories { get; set; }
        public IReadOnlyList<IGradeOption> Options { get; set; }
        public IReadOnlyList<IGradeEquipmentItem> Rest { get; set; }
    }
}