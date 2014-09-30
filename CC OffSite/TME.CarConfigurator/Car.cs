using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator
{
    public class Car : ICar
    {
        public Guid ID { get; private set; }
        public string InternalCode { get; private set; }
        public string LocalCode { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string FootNote { get; private set; }
        public string ToolTip { get; private set; }
        public int SortIndex { get; private set; }
        public IEnumerable<ILabel> Labels { get; private set; }
        public int ShortID { get; set; }
        public bool Promoted { get; set; }
        public bool WebVisible { get; set; }
        public bool ConfigVisible { get; set; }
        public bool FinanceVisible { get; set; }
        public IPrice BasePrice { get; set; }
        public IPrice StartingPrice { get; set; }
        public IBodyType BodyType { get; set; }
        public IEngine Engine { get; set; }
    }
}