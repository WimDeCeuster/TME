
namespace TME.CarConfigurator.Interfaces.Core
{
    public interface IPrice
    {
        decimal PriceInVat { get; }
        decimal PriceExVat { get; }
    }
}
