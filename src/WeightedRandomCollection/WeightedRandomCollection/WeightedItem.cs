using System.Linq;
using System;

namespace WeightedRandomCollection
{
    public class WeightedItem<T>
    {
        public T Item { get; private set; }

        public int Weight { get; private set; }

        public int WeightKey { get; internal set; }

        public WeightedItem(T item, int weight)
        {
            if (weight < 0)
                throw new Exception("Weight cannot be negative");

            //Auto adding weight is forbidden!
            /*            if (weight == 0)
                weight = 1;*/


            Item = item;
            Weight = weight;
        }
    }
}
