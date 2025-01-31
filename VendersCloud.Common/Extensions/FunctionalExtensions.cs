using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace VendersCloud.Common.Common.Extensions {
    public static class FunctionalExtensions {
        public static object CombineProbs(this object obj, object another) {
            var objProps = obj.GetType().GetProperties();
            ExpandoObject newObj = new ExpandoObject();
            foreach (var prop in objProps) {
                AddProperty(newObj, prop.Name, prop.GetValue(obj));
            }
            var anotherProps = another.GetType().GetProperties();
            foreach (var prop in anotherProps) {
                AddProperty(newObj, prop.Name, prop.GetValue(another));
            }
            return newObj;
        }

        public static void AddMissingPropertiesIfNotNull(this Dictionary<string, object> desc, object src) {
            var srcProperties = src.GetType().GetProperties();
            foreach (var prop in srcProperties) {
                var propertyValue = prop.GetValue(src);
                if (propertyValue != null) {
                    AddMissingProperty(desc, prop.Name, propertyValue);
                }
            }
        }

        public static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue) {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }

        public static void AddMissingProperty(Dictionary<string, object> desc, string propertyName, object propertyValue) {
            // ExpandoObject supports IDictionary so we can extend it like this
            if (desc.ContainsKey(propertyName))
                desc[propertyName] = propertyValue;
            else
                desc.Add(propertyName, propertyValue);
        }

        public static object GetPropertyValue(this Dictionary<string, object> desc, string propertyName) {
            // if property  exist then get its value otherwise return null
            var key = desc.Keys.FirstOrDefault(x => x.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
            return key != null ? desc[key] : null;
        }  
        
        public static object GetPropertyValue(this Dictionary<string, object> desc, object propertyName) {
            // if property  exist then get its value otherwise return null
            var key = desc.Keys.FirstOrDefault(x => object.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
            return key != null ? desc[key] : null;
        }
        
        public static object GetPropertyValue(this Dictionary<int, string> desc, object propertyName) {
            // if property  exist then get its value otherwise return null
            var key = desc.Keys.FirstOrDefault(x => object.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
            return key != null ? desc[key] : null;
        }
        public static TValue GetValueOrDefaultTvValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (key == null) throw new ArgumentNullException(nameof(key));

            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }
        

        public static IEnumerable<TSource> IntersectBy<TSource, TKey>(this IEnumerable<TSource> list1, IEnumerable<TSource> list2, Func<TSource, TKey> keySelector) {
            var comparer = new KeyEqualityComparer<TSource, TKey>(keySelector);
            return list1.Intersect(list2, comparer);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) {
            var seenKeys = new HashSet<TKey>();
            foreach (var element in source) {
                if (seenKeys.Add(keySelector(element))) {
                    yield return element;
                }
            }
        }

        private class KeyEqualityComparer<TSource, TKey> : IEqualityComparer<TSource> {
            private readonly Func<TSource, TKey> keySelector;

            public KeyEqualityComparer(Func<TSource, TKey> keySelector) {
                this.keySelector = keySelector;
            }

            public bool Equals(TSource x, TSource y) {
                if (x == null || y == null) return false;
                return EqualityComparer<TKey>.Default.Equals(keySelector(x), keySelector(y));
            }

            public int GetHashCode(TSource obj) {
                if (obj == null) return 0;
                var key = keySelector(obj);
                return key == null ? 0 : key.GetHashCode();
            }
        }

    }
}
