using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class GradeBuilder
    {
        private readonly Grade _grade;

        public GradeBuilder()
        {
            _grade = new Grade();
        }

        public GradeBuilder WithId(Guid id)
        {
            _grade.ID = id;

            return this;
        }

        public GradeBuilder WithLabels(params Repository.Objects.Core.Label[] labels)
        {
            _grade.Labels = labels.ToList();
         
            return this;
        }

        public GradeBuilder WithStartingPrice(Price price)
        {
            _grade.StartingPrice = price;

            return this;
        }

        public GradeBuilder WithBasedUponGradeID(Guid guid)
        {
            _grade.BasedUponGradeID = guid;

            return this;
        }

        public Grade Build()
        {
            return _grade;
        }
    }
}
