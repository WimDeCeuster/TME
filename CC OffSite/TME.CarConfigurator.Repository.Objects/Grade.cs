using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Repository.Objects
{
    
    public class Grade : BaseObject
    {
        
        public bool Special { get; set; }
        public GradeInfo BasedUpon { get; set; }
        public Price StartingPrice { get; set; }
        
        public List<VisibleInModeAndView> VisibleIn { get; set; }

    }
}
