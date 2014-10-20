using System;
using System.Linq;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class SteeringBuilder
    {
        private readonly Steering _steering;

        public SteeringBuilder()
        {
            _steering = new Steering();
        }

        public SteeringBuilder WithId(Guid id)
        {
            _steering.ID = id;

            return this;
        }

        public SteeringBuilder WithLabels(params Repository.Objects.Core.Label[] labels)
        {
            _steering.Labels = labels.ToList();
         
            return this;
        }

        public Steering Build()
        {
            return _steering;
        }
    }
}
