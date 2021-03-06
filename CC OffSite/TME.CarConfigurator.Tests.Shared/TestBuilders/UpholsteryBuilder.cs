﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Colours;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class UpholsteryBuilder
    {
        readonly Upholstery _upholstery;

        public UpholsteryBuilder()
        {
            _upholstery = new Upholstery();
        }

        public UpholsteryBuilder WithId(Guid id)
        {
            _upholstery.ID = id;

            return this;
        }

        public UpholsteryBuilder WithUpholsteryType(UpholsteryType type)
        {
            _upholstery.Type = type;

            return this;
        }

        public UpholsteryBuilder AddVisibleIn(String mode, String view)
        {
            if (_upholstery.VisibleIn == null)
                _upholstery.VisibleIn = new List<VisibleInModeAndView>();

            _upholstery.VisibleIn.Add(new VisibleInModeAndView { Mode = mode, View = view, CanHaveAssets = true});

            return this;
        }

        public Upholstery Build()
        {
            return _upholstery;
        }
    }
}
