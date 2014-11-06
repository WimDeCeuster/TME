using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Core;
using TME.CarConfigurator.Repository.Objects.Packs;

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
            if (guid == Guid.Empty)
            {
                _grade.BasedUpon = null;
                return this;
            }

            _grade.BasedUpon = new GradeInfo() {ID = guid};

            return this;
        }

        public GradeBuilder AddVisibleIn(string mode, string view)
        {
            if (_grade.VisibleIn == null)
                _grade.VisibleIn = new List<VisibleInModeAndView>();

            _grade.VisibleIn.Add(new VisibleInModeAndView() { Mode = mode, View = view });

            return this;
        }

        public Grade Build()
        {
            return _grade;
        }
    }
}
