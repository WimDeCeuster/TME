using System;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class GradeOptionBuilder
    {
        private readonly GradeOption _gradeOption;

        public GradeOptionBuilder()
        {
            _gradeOption = new GradeOption();
        }

        public GradeOptionBuilder WithId(Guid id)
        {
            _gradeOption.ID = id;

            return this;
        }

        public GradeOption Build()
        {
            return _gradeOption;
        }
    }
}