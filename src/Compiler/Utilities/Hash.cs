using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Mango.Compiler.Utilities
{
    internal static class Hash
    {
        internal static int Combine(int newKey, int currentKey)
        {
            return unchecked((currentKey * (int)0xA5555529) + newKey);
        }

        internal static int Combine(bool newKeyPart, int currentKey)
        {
            return Combine(currentKey, newKeyPart ? 1 : 0);
        }

        internal static int Combine<T>(T newKeyPart, int currentKey) where T : class
        {
            var hash = unchecked(currentKey * (int)0xA5555529);

            if (newKeyPart != null)
            {
                return unchecked(hash + newKeyPart.GetHashCode());
            }

            return hash;
        }

        internal static int CombineValues<T>(IEnumerable<T> values, int maxItemsToHash = int.MaxValue)
        {
            if (values == null)
            {
                return 0;
            }

            var hashCode = 0;
            var count = 0;

            foreach (var value in values)
            {
                if (count++ >= maxItemsToHash)
                {
                    break;
                }

                if (value != null)
                {
                    hashCode = Combine(value.GetHashCode(), hashCode);
                }
            }

            return hashCode;
        }

        internal static int CombineValues<T>(T[] values, int maxItemsToHash = int.MaxValue)
        {
            if (values == null)
            {
                return 0;
            }

            var maxSize = Math.Min(maxItemsToHash, values.Length);
            var hashCode = 0;

            for (var i = 0; i < maxSize; i++)
            {
                var value = values[i];

                if (value != null)
                {
                    hashCode = Combine(value.GetHashCode(), hashCode);
                }
            }

            return hashCode;
        }

        internal static int CombineValues<T>(ImmutableArray<T> values, int maxItemsToHash = int.MaxValue)
        {
            if (values.IsDefaultOrEmpty)
            {
                return 0;
            }

            var hashCode = 0;
            var count = 0;

            foreach (var value in values)
            {
                if (count++ >= maxItemsToHash)
                {
                    break;
                }

                if (value != null)
                {
                    hashCode = Combine(value.GetHashCode(), hashCode);
                }
            }

            return hashCode;
        }
    }
}
