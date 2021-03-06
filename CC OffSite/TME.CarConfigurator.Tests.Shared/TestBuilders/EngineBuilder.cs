﻿using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class EngineBuilder
    {
        private readonly Engine _engine;

        public EngineBuilder()
        {
            _engine = new Engine();
        }

        public EngineBuilder WithId(Guid id)
        {
            _engine.ID = id;

            return this;
        }

        public EngineBuilder WithLabels(params Repository.Objects.Core.Label[] labels)
        {
            _engine.Labels = labels.ToList();
         
            return this;
        }

        public EngineBuilder WithCategory(EngineCategory category)
        {
            _engine.Category = category;

            return this;
        }

        public EngineBuilder WithType(EngineType engineType)
        {
            _engine.Type = engineType;

            return this;
        }

        public EngineBuilder AddVisibleIn(string mode , string view)
        {
            if (_engine.VisibleIn == null) 
                _engine.VisibleIn =  new List<VisibleInModeAndView>();
            
            _engine.VisibleIn.Add(new VisibleInModeAndView() { Mode = mode, View = view, CanHaveAssets = true});
            
            return this;
        }

        public Engine Build()
        {
            return _engine;
        }
    }
}
