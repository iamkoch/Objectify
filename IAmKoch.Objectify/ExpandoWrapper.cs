using System;
using System.Dynamic;
using System.Linq;
using System.Text.Json.Serialization;

namespace IAmKoch.ObjectAssign
{
    internal class ExpandoWrapper<T> : ExpandoWrapper
    {

        private static T Clone<T>(T item)
        {
            return JsonSerializer.Parse<T>(JsonSerializer.ToString(item));
        }

        public ExpandoWrapper(object item) : base(Clone(item))
        {
        }

        public override bool TryConvert(
            ConvertBinder binder, out object result)
        {
            // Converting to string.
            if (binder.Type == typeof(T))
            {
                var properties = typeof(T).GetProperties();
                var instance = Activator.CreateInstance<T>();
                foreach (var keyValuePair in Properties)
                {
                    var matchingPropertyByName = properties.FirstOrDefault(x => x.Name.ToLowerInvariant() == keyValuePair.Key);
                    if (matchingPropertyByName != null)
                        matchingPropertyByName.SetValue(instance, keyValuePair.Value);
                }

                result = instance;

                return true;
            }

            // In case of any other type, the binder
            // attempts to perform the conversion itself.
            // In most cases, a run-time exception is thrown.
            return base.TryConvert(binder, out result);
        }
    }
}