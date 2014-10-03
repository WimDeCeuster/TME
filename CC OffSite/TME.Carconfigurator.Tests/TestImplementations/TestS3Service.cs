using System;
using System.Collections.Generic;
using TME.CarConfigurator.Publisher.Enums.Result;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.Carconfigurator.Tests.TestImplementations
{
    public class TestS3Service : IS3Service
    {
        public readonly Dictionary<String, String> Content = new Dictionary<String, String>();

        public void PutObject(string key, string item)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (item == null) throw new ArgumentNullException("item");

            if (String.IsNullOrWhiteSpace(key)) throw new ArgumentException("key cannot be empty", "key");
            if (String.IsNullOrWhiteSpace(item)) throw new ArgumentException("item cannot be empty", "item");

            if (Content.ContainsKey(key)) throw new ArgumentException("key has already been published by this service", "key");

            Content[key] = item;
        }

        public Models GetModelsOverview(string brand, string country, string language)
        {
            return null;
        }

        public Result PutModelsOverview(string brand, string country, string language, Models models)
        {
            return null;
        }
    }
}
