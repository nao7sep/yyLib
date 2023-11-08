using System.Collections;

namespace yyLib
{
    public class yyAutoExpandingList <T>: IEnumerable <T> where T: new ()
    {
        public List <T> List { get; } = new List <T> ();

        public int Count => List.Count;

        private void EnsureCapacity (int capacity)
        {
            while (List.Count < capacity)
                List.Add (new T ());
        }

        public T this [int index]
        {
            get
            {
                EnsureCapacity (index + 1);
                return List [index];
            }

            set
            {
                EnsureCapacity (index + 1);
                List [index] = value;
            }
        }

        public bool Contains (T item) => List.Contains (item);

        public void Add (T item) => List.Add (item);

        public IEnumerator <T> GetEnumerator () => List.GetEnumerator ();

        IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();

        public void CopyTo (T [] array, int arrayIndex) => List.CopyTo (array, arrayIndex);

        public bool Remove (T item) => List.Remove (item);

        public void Clear () => List.Clear ();
    }
}
