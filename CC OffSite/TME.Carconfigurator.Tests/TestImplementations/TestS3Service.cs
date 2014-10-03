using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher;
using TME.CarConfigurator.Publisher.Interfaces;

namespace TME.Carconfigurator.Tests.TestImplementations
{
    public class TestS3Service : IS3Service
    {
        public readonly Dictionary<String, String> Content = new Dictionary<String, String>();

        public void PutObject(String key, String item)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (item == null) throw new ArgumentNullException("item");

            if (String.IsNullOrWhiteSpace(key)) throw new ArgumentException("key cannot be empty", "key");
            if (String.IsNullOrWhiteSpace(item)) throw new ArgumentException("item cannot be empty", "item");

            if (Content.ContainsKey(key)) throw new ArgumentException("key has already been published by this service", "key");

            Content[key] = item;
        }
    }
}
