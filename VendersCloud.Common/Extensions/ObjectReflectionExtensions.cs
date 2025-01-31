using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace VendersCloud.Common.Extensions {
    public static class ObjectReflectionExtensions {
        public static object GetPropValue(this object obj, string propName) {
            var prop = obj.GetType().GetProperty(propName);
            if(prop != null) {
                return prop.GetValue(obj);
            }
            else {
                var props = obj as IEnumerable<KeyValuePair<string, object>>;
                var p = props.FirstOrDefault(f => f.Key == propName) as Nullable<KeyValuePair<string, object>>;
                return p?.Value;
            }
        }

        public static T DeepClone<T>(this T obj) {
            var json = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }

    public static class ObjectUpdater<T> {
        public static void Update(T existingObject, T newObject) {
            Type objectType = typeof(T);

            foreach (PropertyInfo property in objectType.GetProperties()) {
                if (property.Name == "Id") continue;
                if (property.PropertyType == typeof(string)) {
                    string newValue = (string)property.GetValue(newObject);
                    if (!string.IsNullOrWhiteSpace(newValue)) {
                        property.SetValue(existingObject, newValue);
                    }
                }

                if (property.PropertyType == typeof(bool)) {
                    bool newValue = (bool)property.GetValue(newObject);
                    if(property.GetValue(newObject) != null)
                        property.SetValue(existingObject, newValue);
                }
                if (property.PropertyType == typeof(int)) {
                    int newValue = (int)property.GetValue(newObject);
                    if(property.GetValue(newObject) != null)
                        property.SetValue(existingObject, newValue);
                }
            }
        }
    }
}
