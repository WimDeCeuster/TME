using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.FrontEndViewer.Models
{
    public class BaseObjectWithPrefix
    {
        public IBaseObject BaseObject { get; set; }
        public string Prefix { get; set; }

    }
}