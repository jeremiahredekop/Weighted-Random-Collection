using System;
using System.Linq;

namespace WeightedRandomCollection.Support
{
    internal class RandomHelper
    {
        private static Random _random;

        internal static int GetRandomNumber(int minValue, int maxValue)
        {
            if (_random == null)
                _random = new Random();
            return _random.Next(minValue, maxValue) + 1;
        }

        internal static int GetRandomNumber(int maxValue)
        {
            return GetRandomNumber(0, maxValue);
        }
    }
}