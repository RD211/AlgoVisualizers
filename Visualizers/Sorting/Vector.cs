using System;
using System.Collections.Generic;
using System.Linq;


namespace Visualizers
{
    public class Vector<T> : List<T> where T: IComparable
    {
        Queue<int> SelectedItems = new Queue<int>();
        public T this[int i]
        {
            get => base[i];
            set
            {
                if (SelectedItems.Count > 1)
                    SelectedItems.Dequeue();
                SelectedItems.Enqueue(i);
                base[i] = value;
            }
        }

        public void Shuffle()
        {
            List<T> shuffledList = this.OrderBy(a => Guid.NewGuid()).ToList();
            for(int i = 0;i<this.Count;i++)
                base[i] = shuffledList[i];
        }
        public bool isSelected(int i)
        {
            foreach(int x in SelectedItems)
            {
                if (x == i)
                    return true;
            }
            return false;
        }
    }
}
