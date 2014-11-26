using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.Publisher.Helpers
{
    public class Comparer<T> : IEqualityComparer<T>
    {
        Func<T, T, bool> _equals;
        Func<T, object> _member;
        Func<T, int> _hash;

        public Comparer(Func<T, object> member)
            : this((x, y) => Object.Equals(member(x), member(y)), null)
        {
            _member = member;
        }

        public Comparer(Func<T, T, bool> equals, Func<T, int> hash)
        {
            _equals = equals;
            _hash = hash;
        }

        public bool Equals(T x, T y)
        {
            return _equals(x, y);
        }

        public int GetHashCode(T obj)
        {
            if (_member != null)
                return _member(obj).GetHashCode();
            return _hash(obj);
        }
    }
}
