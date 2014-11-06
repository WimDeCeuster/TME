using System;
using System.Collections.Generic;

namespace TME.CarConfigurator.Repository.Objects.Core
{
    
    public abstract class BaseObject
    {
        protected BaseObject()
        {
            Labels = new List<Label>();
        }
        
        public Guid ID { get; set; }
        
        public string InternalCode { get; set; }
        
        public string LocalCode { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public string FootNote { get; set; }
        
        public string ToolTip { get; set; }
        
        public int SortIndex { get; set; }
        
        public IReadOnlyList<Label> Labels { get; set; }
    }
}
