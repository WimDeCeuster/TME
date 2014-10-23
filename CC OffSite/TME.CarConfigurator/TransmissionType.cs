using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;

namespace TME.CarConfigurator
{
    public class TransmissionType : BaseObject<Repository.Objects.TransmissionType>, ITransmissionType
    {
        public TransmissionType(Repository.Objects.TransmissionType repositoryTransmissionType)
            : base(repositoryTransmissionType)
        {
        }
    }
}