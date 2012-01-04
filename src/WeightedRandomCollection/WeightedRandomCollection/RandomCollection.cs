using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WeightedRandomCollection.Support;

namespace WeightedRandomCollection
{
    public class RandomCollection<T> : IEnumerable<WeightedItem<T>>, IEnumerable<T>
    {
        protected internal Dictionary<int, WeightedItem<T>> Items;
        private int _totalWeight;

        public int TotalNumberOfItems
        {
            get
            {
                return Items.Count;
            }
        }

        public int TotalWeight
        {
            get
            {
                return _totalWeight;
            }
        }

        public RandomCollection(IEnumerable<WeightedItem<T>> items)
        {
            SetWeightedItems(items);
        }

        private RandomCollection(Dictionary<int, WeightedItem<T>> itemDictionary)
        {
            Items = itemDictionary;
            _totalWeight = itemDictionary.Values.Sum(a => a.Weight);
        }

        protected virtual List<WeightedItem<T>> OnPickUniqueRandomValues(int desiredNumber, bool performUniqueTest)
        {
            List<WeightedItem<T>> output;
            TryPickUniqueRandomValues(desiredNumber, performUniqueTest, out output);
            return output;
        }

        public bool TryPickUniqueRandomValues(int desiredNumber, bool performUniqueTest, out List<WeightedItem<T>> output)
        {
            if (desiredNumber < 1)
                throw new Exception("Count must be greater than 0");
            if (desiredNumber >= TotalNumberOfItems)
            {
                output = AsWeightedItems().ToList();
                return desiredNumber == TotalNumberOfItems;
            }
            
            output = new List<WeightedItem<T>>();
            var myList = this;
            
            for (var index = 0; index < desiredNumber; ++index)
            {
                var newItem = GetNewItem(performUniqueTest, ref myList);
                output.Add(newItem);
            }

            if (performUniqueTest && output.DistinctBy(o => o.WeightKey).Count() != output.Count)
                throw new Exception("Values returned were not random, duplicates found");
            
            return true;
        }

        public WeightedItem<T> PickClosestItem(int value)
        {
            return Items.GetClosestMatch(value, _totalWeight);
        }

        public List<WeightedItem<T>> PickUniqueRandomValues(int desiredNumber, bool performUniqueTest)
        {
            return OnPickUniqueRandomValues(desiredNumber, performUniqueTest);
        }

        public List<WeightedItem<T>> PickUniqueRandomValues(int count)
        {
            const bool performUniqueTest = false;
            return PickUniqueRandomValues(count, performUniqueTest);
        }

        public WeightedItem<T> PickRandomItem()
        {
            return PickClosestItem(RandomHelper.GetRandomNumber(_totalWeight));
        }

        public void SetWeightedItems(IEnumerable<WeightedItem<T>> items)
        {
            Items = items.Where(a=> a.Weight > 0).ToArray().ToWeightedDictionary(out _totalWeight);
        }

        public IEnumerable<WeightedItem<T>> AsWeightedItems()
        {
            return this;
        }

        public IEnumerable<T> AsItems()
        {
            return this;
        }

        public void CheckIntegrity()
        {
            Items.ToList().ForEach(i =>
                                                  {
                                                      if (i.Key == i.Value.WeightKey)
                                                          return;
                                                      throw new Exception(String.Format("Integrity error on random collection.  DictonaryKey was <{0}>, WeightKey was <{1}>", i.Key, i.Value.WeightKey));
                                                  });
        }

        private RandomCollection<T> GetNewRandomCollectionExcludingWeightedItem(WeightedItem<T> item)
        {
            var itemDictionary = new Dictionary<int, WeightedItem<T>>(Items);
            itemDictionary.Remove(item.WeightKey);
            return new RandomCollection<T>(itemDictionary);
        }

        private static WeightedItem<T> GetNewItem(bool performUniqueTest, ref RandomCollection<T> myList)
        {
            var newItem = myList.PickRandomItem();

            myList = myList.GetNewRandomCollectionExcludingWeightedItem(newItem);
            if (performUniqueTest)
            {
                myList.CheckIntegrity();
                if (myList.AsWeightedItems().Any(a => a.WeightKey == newItem.WeightKey))
                    throw new Exception("Uhoh!");
            }
            return newItem;
        }

        public IEnumerator<WeightedItem<T>> GetEnumerator()
        {
            return Items.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return Items.Values.Select(o => o.Item).GetEnumerator();
        }
    }
}
