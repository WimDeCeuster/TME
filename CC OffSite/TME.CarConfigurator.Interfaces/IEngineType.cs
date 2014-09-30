namespace TME.CarConfigurator.Interfaces
{
    public interface IEngineType 
    {
        string Code { get; }
        string Name { get; }

        IFuelType FuelType { get; }
    }
}