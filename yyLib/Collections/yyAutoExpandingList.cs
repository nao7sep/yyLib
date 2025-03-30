using System.Collections;

namespace yyLib
{
    public class yyAutoExpandingList <ItemType>: IEnumerable <ItemType> where ItemType: new ()
    {
        public IList <ItemType> Items { get; } = [];

        public int Count => Items.Count;

        private void _EnsureCapacity (int capacity)
        {
            while (Items.Count < capacity)
                Items.Add (new ());
        }

        public ItemType this [int index]
        {
            get
            {
                _EnsureCapacity (index + 1);
                return Items [index];
            }

            set
            {
                _EnsureCapacity (index + 1);
                Items [index] = value;
            }
        }

        public bool Contains (ItemType item) => Items.Contains (item);

        public void Add (ItemType item) => Items.Add (item);

        public IEnumerator <ItemType> GetEnumerator () => Items.GetEnumerator ();

        IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();

        public void CopyTo (ItemType [] array, int arrayIndex) => Items.CopyTo (array, arrayIndex);

        public bool Remove (ItemType item) => Items.Remove (item);

        public void Clear () => Items.Clear ();
    }
}
