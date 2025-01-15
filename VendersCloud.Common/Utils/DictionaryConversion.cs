using System.ComponentModel;
using System.Reflection;

namespace VendersCloud.Common.Utils
{
    public class DictionaryConversion {
        public static void ConvertDictionaryToObject<T>(T dest, Dictionary<string, object> src) {
            var srcProperties = src.Select(x => x.Key).ToList();
            var destProperties = TypeDescriptor.GetProperties(dest);
            var type = typeof(T);
            //foreach (PropertyDescriptor item in TypeDescriptor.GetProperties(dest))
            foreach (string propertyName in src.Keys) {
                if (src.ContainsKey(propertyName) == false)
                    continue;

                //https://gist.github.com/afreeland/6796800
                PropertyInfo propInfo = typeof(T).GetProperty(propertyName);

                dynamic propValue = src[propertyName];
                // Get the type code so we can switch
                // Gets what the data type is of our property (Foreign Key Property)
                Type propertyType = propInfo.PropertyType;
                TypeCode typeCode = System.Type.GetTypeCode(propertyType);
                // Get the type code so we can switch
                try {
                    switch (typeCode) {
                        case TypeCode.Int32:
                            propInfo.SetValue(dest, Convert.ToInt32(propValue), null);
                            break;
                        case TypeCode.Int64:
                            propInfo.SetValue(dest, Convert.ToInt64(propValue), null);
                            break;
                        case TypeCode.String:
                            propInfo.SetValue(dest, propValue, null);
                            break;
                        case TypeCode.DateTime:
                            propInfo.SetValue(dest, propValue, null);
                            break;
                        case TypeCode.Object:
                            if (propertyType == typeof(Guid) || propertyType == typeof(Guid?)) {
                                propInfo.SetValue(dest, Guid.Parse(propValue.ToString()), null);
                                return;
                            }
                            else if (propertyType == typeof(double) || propertyType == typeof(double?)) {
                                propInfo.SetValue(dest, double.Parse(propValue.ToString()), null);
                                return;
                            }
                            else if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?)) {
                                propInfo.SetValue(dest, DateTime.Parse(propValue.ToString()), null);
                                return;
                            }
                            break;
                        default:
                            propInfo.SetValue(dest, propValue, null);
                            break;
                    }
                }
                catch (Exception ex) {
                    throw new Exception("Failed to set property value for our Foreign Key");
                }
            }
        }

        public static Dictionary<string, Object> ConvertObjectToDictionary<T>(T src) {
            var objectAsDict = new Dictionary<string, object>();
            var destProperties = TypeDescriptor.GetProperties(src);
            var type = typeof(T);
            foreach (PropertyDescriptor prop in destProperties) {
                PropertyInfo propInfo = type.GetProperty(prop.Name);
                if (propInfo != null && objectAsDict.ContainsKey(prop.Name) == false) {
                    var value = propInfo.GetValue(src, null);
                    objectAsDict.Add(prop.Name, value);
                }
            }

            return objectAsDict;
        }
    }
}
