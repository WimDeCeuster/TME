using System;
using System.Collections.Generic;

namespace TME.CarConfigurator.Repository.Objects
{
    public class Languages : List<Language>
    {

    }

    public class Language
    {
        public Language(string language)
        {
            Code = language;
            Models = new List<Model>();
        }

        public string Code { get; private set; }
        public List<Model> Models { get; set; }
    }
}