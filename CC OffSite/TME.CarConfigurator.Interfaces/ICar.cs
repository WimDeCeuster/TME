using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator.Interfaces
{
    public interface ICar : IBaseObject
    {
        int ShortID { get; set; }
        bool Promoted { get; set; }

        bool WebVisible { get; set; }
        bool ConfigVisible { get; set; }
        bool FinanceVisible { get; set; }
        IPrice BasePrice { get; set; }
        IPrice StartingPrice { get; set; }
        
        IBodyType BodyType { get; set; }
        IEngine Engine { get; set; }
  }
}
