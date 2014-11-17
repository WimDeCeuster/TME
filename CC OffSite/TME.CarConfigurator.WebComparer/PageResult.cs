using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TME.CarConfigurator.WebComparer
{
    public class PageResult
    {
        public String Url { get; private set; }
        public IList<String> MissingObjects { get; set; }
        public IList<String> WrongOrderedObjects { get; set; }
        public IList<String> MismatchedProperties { get; set; }

        public bool IsValid
        {
            get
            {
                return 0 == (MissingObjects == null ? 0 : MissingObjects.Count) +
                            (WrongOrderedObjects == null ? 0 : WrongOrderedObjects.Count) +
                            (MismatchedProperties == null ? 0 : MismatchedProperties.Count);
            }
        }

        public PageResult(string url)
        {
            Url = url;

            MissingObjects = new List<string>();
            WrongOrderedObjects = new List<string>();
            MismatchedProperties = new List<string>();
        }
    }
}
