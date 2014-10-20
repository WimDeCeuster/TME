using System;
using System.Collections.Generic;

namespace TME.CarConfigurator.Repository.Objects.Core
{
    
    public class BaseObject
    {
        
        public Guid ID { get; set; }
        
        public string InternalCode { get; set; }
        
        public string LocalCode { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public string FootNote { get; set; }
        
        public string ToolTip { get; set; }
        
        public int SortIndex { get; set; }
        
        public List<Label> Labels { get; set; }
    }
}
