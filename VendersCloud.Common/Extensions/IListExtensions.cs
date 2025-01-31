using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VendersCloud.Common.Extensions {
    public static class IListExtensions {
        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable {
            return listToClone.Select(item => (T) item.Clone()).ToList();
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> list, int parts) {
            int i = 0;
            var splits = from item in list
                group item by i++ % parts
                into part
                select part.AsEnumerable();
            return splits;
        }

        /// <summary>
        /// Split list into smaller lists of N size
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="locations"></param>
        /// <param name="nSize"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> SplitList<T>(this IEnumerable<T> locations, int nSize = 200) {
            for(int i = 0 ; i < locations.Count() ; i += nSize) {
                yield return locations.ToList().GetRange(i, Math.Min(nSize, locations.Count() - i));
            }
        }
    }
}
