using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class TransmissionBuilder
    {
        private readonly Transmission _transmission;

        public TransmissionBuilder()
        {
            _transmission = new Transmission();
        }

        public TransmissionBuilder WithId(Guid id)
        {
            _transmission.ID = id;

            return this;
        }

        public TransmissionBuilder WithType(TransmissionType type)
        {
            _transmission.Type = type;

            return this;
        }

        public TransmissionBuilder WithLabels(params Repository.Objects.Core.Label[] labels)
        {
            _transmission.Labels = labels.ToList();
         
            return this;
        }

        public Transmission Build()
        {
            return _transmission;
        }
    }
}
