using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class WheelDriveBuilder
    {
        private readonly WheelDrive _wheelDrive;

        public WheelDriveBuilder()
        {
            _wheelDrive = new WheelDrive();
        }

        public WheelDriveBuilder WithId(Guid id)
        {
            _wheelDrive.ID = id;

            return this;
        }

        public WheelDriveBuilder WithLabels(params Repository.Objects.Core.Label[] labels)
        {
            _wheelDrive.Labels = labels.ToList();
         
            return this;
        }

        public WheelDriveBuilder AddVisibleIn(string mode, string view)
        {
            if (_wheelDrive.VisibleIn == null)
                _wheelDrive.VisibleIn = new List<VisibleInModeAndView>();

            _wheelDrive.VisibleIn.Add(new VisibleInModeAndView() { Mode = mode, View = view });

            return this;
        }

        public WheelDrive Build()
        {
            return _wheelDrive;
        }
    }
}
