using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TME.CarConfigurator.Repository.Objects.Core
{
    [DataContract]
    public class BaseObject
    {
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public string InternalCode { get; set; }
        [DataMember]
        public string LocalCode { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string FootNote { get; set; }
        [DataMember]
        public string ToolTip { get; set; }
        [DataMember]
        public int SortIndex { get; set; }
        [DataMember]
        public List<Label> Labels { get; set; }
    }
}
