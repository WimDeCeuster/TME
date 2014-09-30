using System.Collections;
using System.Collections.Generic;

namespace TME.CarConfigurator.Core
{
    public abstract class ReadOnlyList<T> : IReadOnlyList<T>
    {
        private List<T> _list = new List<T>();
        protected List<T> List
        {
            get { return _list; }
            set { _list = value; }
        }

        public T this[int index]
        {
            get { return List[index]; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return List.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count { get { return List.Count; } }


    }
}
