using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator
{
    public class OptionInfo : IOptionInfo
    {
        public OptionInfo(int shortId, string name)
        {
            ShortID = shortId;
            Name = name;
        }

        public int ShortID { get; private set; }

        public string Name { get; private set; }
    }
}
