using System;
using System.Collections.Generic;
using System.Linq;
using WeightedRandomCollection.Support;

namespace WeightedRandomCollection
{
    public static class RandomCollectionExtensions
    {

        public static RandomCollection<T> ToRandomCollection<T>(this IEnumerable<T> input, Func<T, int> getWeight)
        {
            return new RandomCollection<T>(input.Select(t => new WeightedItem<T>(t, getWeight(t))));
        }

        public static Dictionary<int, WeightedItem<T>> ToWeightedDictionary<T>(this IList<WeightedItem<T>> items, out int weightTotal)
        {
            var num = 0;
            var dictionary = new Dictionary<int, WeightedItem<T>>(items.Count);
            foreach (var weightedItem in items)
            {
                var key = num + weightedItem.Weight;
                weightedItem.WeightKey = key;
                dictionary.Add(key, weightedItem);
                num = key;
            }
            weightTotal = num;
            return dictionary;
        }

        public static WeightedItem<T> GetClosestMatch<T>(this Dictionary<int, WeightedItem<T>> items, int value, int totalWeight)
        {
            if (value < 0)
                throw new Exception("Value may not be negative");
            if (value > totalWeight)
                throw new Exception("Not enough weight in collection for value");
            WeightedItem<T> weightedItem;
            
            return items.TryGetValue(value, out weightedItem) ? weightedItem : items.Where(t => value <= t.Key).OrderBy(t => t.Key).First().Value;
        }

        public static WeightedItem<T> PickRandomItem<T>(this Dictionary<int, WeightedItem<T>> items, int totalWeight)
        {
            var randomNumber = RandomHelper.GetRandomNumber(totalWeight);
            return GetClosestMatch(items, randomNumber, totalWeight);
        }
    }

    public static class CollectionExtensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

    }

}
