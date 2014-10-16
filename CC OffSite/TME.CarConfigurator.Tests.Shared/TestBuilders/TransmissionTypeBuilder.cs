using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class TransmissionTypeBuilder
    {
        private readonly TransmissionType _transmissionType;

        public TransmissionTypeBuilder()
        {
            _transmissionType = new TransmissionType();
        }

        public TransmissionTypeBuilder WithId(Guid id)
        {
            _transmissionType.ID = id;

            return this;
        }

        public TransmissionTypeBuilder WithLabels(params Repository.Objects.Core.Label[] labels)
        {
            _transmissionType.Labels = labels.ToList();
         
            return this;
        }

        public TransmissionType Build()
        {
            return _transmissionType;
        }
    }
}
