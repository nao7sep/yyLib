using System.Collections;

namespace yyLib
{
    public class yyToggleList <T>: IList <T> where T: yyIToggleable
    {
        private List <T> _items = [];

        // -----------------------------------------------------------------------------
        // All Items
        // -----------------------------------------------------------------------------

        public List <T> AllItems => _items;

        // ChatGPT says "FieldCount", "RecentLogCount", "interactionCount", etc dont need to be pluralized while "AllItemsCount" does.
        // I asked why and, as one of its answers, I got the following explanation.
        // So far, this is the most persuasive explanation to me.

        // In C# and other programming languages, variable names are often crafted to clearly reflect their purpose and the data they represent.
        // When using the word 'all' in a variable name such as 'AllItemsCount', the implication is that the count includes every item in a collection.
        // The word 'all' naturally suggests a plurality, hence 'Items' is used in the plural form. This naming convention indicates that we are dealing with
        // multiple items as a group rather than individually.

        // Such naming practices ensure that the intent behind the variable is instantly clear to anyone reading the code. For example, 'AllItemsCount' tells
        // the programmer that this variable is used to store the total number of items, encompassing all individual elements within the collection. This clarity
        // helps in maintaining and understanding code, especially in collaborative environments or when revisiting older code.

        public int AllItemsCount => _items.Count;

        // -----------------------------------------------------------------------------
        // "On" Items
        // -----------------------------------------------------------------------------

        // Properties and methods are defined in the same order as in the documentation.
        // https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1

        public int Count => _items.Count (x => x.IsOn);

        public bool IsReadOnly => false;

        public T this [int index]
        {
            get
            {
                int xIndex = 0;

                // We can also compare index with the length of the list,
                // but a situation where the given index is "clearly" out of range should be relatively rare.

                for (int temp = 0; temp < _items.Count; temp ++)
                {
                    if (_items [temp].IsOn)
                    {
                        if (xIndex == index)
                            return _items [temp];

                        xIndex ++;
                    }
                }

                throw new yyArgumentException ("Index out of range.");
            }

            set
            {
                int xIndex = 0;

                for (int temp = 0; temp < _items.Count; temp ++)
                {
                    if (_items [temp].IsOn)
                    {
                        if (xIndex == index)
                        {
                            _items [temp] = value;
                            return;
                        }

                        xIndex ++;
                    }
                }

                throw new yyArgumentException ("Index out of range.");
            }
        }

        public void Add (T item) => _items.Add (item);

        public void Clear () => _items.Clear ();

        public bool Contains (T item) => _items.Any (x => x.IsOn && x.Equals (item));

        public void CopyTo (T [] array, int arrayIndex)
        {
            int xIndex = arrayIndex;

            for (int temp = 0; temp < _items.Count; temp ++)
            {
                if (_items [temp].IsOn)
                {
                    array [xIndex] = _items [temp];
                    xIndex ++;
                }
            }
        }

        // https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/Collections/Generic/List.cs
        // https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1.getenumerator
        // https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable.getenumerator

        // This method implements IEnumerable<T>.GetEnumerator() explicitly.
        // It does not have an access modifier because it's an explicit implementation of an interface method.
        // In C#, explicit interface implementations are not accessible through class instances directly;
        // they can only be called through instances of the interface.
        // This method yields only "on" items, providing a type-safe enumerator for the collection.

        IEnumerator <T> IEnumerable <T>.GetEnumerator ()
        {
            foreach (T item in _items)
            {
                if (item.IsOn)
                    yield return item;
            }
        }

        // This method implements IEnumerable.GetEnumerator() and is public.
        // It can be accessed directly via instances of the ToggleList class.
        // It calls the explicitly implemented IEnumerable<T>.GetEnumerator method above,
        // which simplifies code by reusing the same iteration logic.
        // This method returns a non-generic IEnumerator, making the collection compatible with
        // non-generic interfaces and older APIs that require IEnumerable.

        public IEnumerator GetEnumerator () => ((IEnumerable <T>) this).GetEnumerator ();

        public int IndexOf (T item)
        {
            int xIndex = 0;

            for (int temp = 0; temp < _items.Count; temp ++)
            {
                if (_items [temp].IsOn)
                {
                    if (_items [temp].Equals (item))
                        return xIndex;

                    xIndex ++;
                }
            }

            return -1;
        }

        public void Insert (int index, T item)
        {
            int xIndex = 0;

            for (int temp = 0; temp < _items.Count; temp ++)
            {
                if (_items [temp].IsOn)
                {
                    if (xIndex == index)
                    {
                        _items.Insert (temp, item);
                        return;
                    }

                    xIndex ++;
                }
            }

            throw new yyArgumentException ("Index out of range.");
        }

        public bool Remove (T item)
        {
            for (int temp = 0; temp < _items.Count; temp ++)
            {
                if (_items [temp].IsOn && _items [temp].Equals (item))
                {
                    _items.RemoveAt (temp);
                    return true;
                }
            }

            return false;
        }

        public void RemoveAt (int index)
        {
            int xIndex = 0;

            for (int temp = 0; temp < _items.Count; temp ++)
            {
                if (_items [temp].IsOn)
                {
                    if (xIndex == index)
                    {
                        _items.RemoveAt (temp);
                        return;
                    }

                    xIndex ++;
                }
            }

            throw new yyArgumentException ("Index out of range.");
        }
    }
}
