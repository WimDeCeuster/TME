using System;
namespace TME.CarConfigurator.Repository.Objects
{
    public class Context : IEquatable<Context>
    {
        public string Brand { get; set; }
        public string Country { get; set; }
        public string Language { get; set; }

        public override int GetHashCode()
        {
            var result = Brand != null ? Brand.ToLowerInvariant().GetHashCode() : 0;
            result = 397 * result + (Country != null ? Country.ToLowerInvariant().GetHashCode() : 0);
            result = 397 * result + (Language != null ? Language.ToLowerInvariant().GetHashCode() : 0);
            return result;
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            var other = obj as Context;
            if (other == null) return false;

            if (!String.Equals(Brand, other.Brand, StringComparison.InvariantCultureIgnoreCase)) return false;
            if (!String.Equals(Country, other.Country, StringComparison.InvariantCultureIgnoreCase)) return false;
            if (!String.Equals(Language, other.Language, StringComparison.InvariantCultureIgnoreCase)) return false;

            return true;
        }

        public bool Equals(Context other)
        {
            return Equals((object)other);
        }
    }
}
