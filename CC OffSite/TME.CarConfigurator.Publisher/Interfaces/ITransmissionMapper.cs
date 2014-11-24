using Transmission = TME.CarConfigurator.Repository.Objects.Transmission;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface ITransmissionMapper
    {
        Transmission MapTransmission(Administration.ModelGenerationTransmission transmission, bool canHaveAssets);
    }
}
