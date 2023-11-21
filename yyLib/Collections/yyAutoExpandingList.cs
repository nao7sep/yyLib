using System.Collections;

namespace yyLib
{
    public class yyAutoExpandingList <T>: IEnumerable <T> where T: new ()
    {
        public List <T> Items { get; } = [];

        public int Count => Items.Count;

        private void EnsureCapacity (int capacity)
        {
            while (Items.Count < capacity)
                Items.Add (new T ());
        }

        public T this [int index]
        {
            get
            {
                EnsureCapacity (index + 1);
                return Items [index];
            }

            set
            {
                EnsureCapacity (index + 1);
                Items [index] = value;
            }
        }

        public bool Contains (T item) => Items.Contains (item);

        public void Add (T item) => Items.Add (item);

        public IEnumerator <T> GetEnumerator () => Items.GetEnumerator ();

        IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();

        public void CopyTo (T [] array, int arrayIndex) => Items.CopyTo (array, arrayIndex);

        public bool Remove (T item) => Items.Remove (item);

        public void Clear () => Items.Clear ();
    }
}
