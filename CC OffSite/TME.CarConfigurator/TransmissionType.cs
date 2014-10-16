using System;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;

namespace TME.CarConfigurator
{
    public class TransmissionType : BaseObject, ITransmissionType
    {
        private readonly Repository.Objects.TransmissionType _repositoryTransmissionType;

        public TransmissionType(Repository.Objects.TransmissionType repositoryTransmissionType)
            : base(repositoryTransmissionType)
        {
            if (repositoryTransmissionType == null) throw new ArgumentNullException("repositoryTransmissionType");
            _repositoryTransmissionType = repositoryTransmissionType;
        }
    }
}