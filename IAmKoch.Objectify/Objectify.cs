using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace IAmKoch.ObjectAssign
{
    public static class Objectify
    {
        public static dynamic Assign(object target, params object[] sources)
        {
            var oSrc = new ExpandoWrapper(target);

            foreach (var source in sources)
            {
                var dSrc = new ExpandoWrapper(source);
                foreach (var kv in dSrc.Properties)
                    oSrc[kv.Key] = kv.Value;

            }

            return oSrc;
        }

        public static T Assign<T>(object target, params object[] sources) where T : class
        {
            var instance = Activator.CreateInstance<T>();

            foreach (var source in sources.Prepend(target))
            {
                var dSrc = new ExpandoWrapper(source);

                var properties = typeof(T).GetProperties();
                foreach (var keyValuePair in dSrc.Properties)
                {
                    var matchingPropertyByName = properties.FirstOrDefault(x => x.Name.ToLowerInvariant() == keyValuePair.Key);
                    if (matchingPropertyByName != null)
                        matchingPropertyByName.SetValue(instance, keyValuePair.Value);
                }
            }

            return instance;
        }
    }

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

    internal class ExpandoWrapper : DynamicObject
    {
        private readonly object _item;
        private readonly Dictionary<string, PropertyInfo> _ignoreCaseLookup = new Dictionary<string, PropertyInfo>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, Box> _ignoreCaseLookupExtra = new Dictionary<string, Box>(StringComparer.InvariantCultureIgnoreCase);

        private class Box
        {
            public Box(object item)
            {
                Item = item;
            }

            public object Item { get; }
        }

        public object this[string key]
        {
            get { return Properties.FirstOrDefault(x => x.Key == key).Value; }
            set => AddBox(key, value);
        }

        private void AddBox(string key, object value)
        {
            var box = new Box(value);
            _ignoreCaseLookupExtra[key.ToLowerInvariant()] = box;
        }

        public ExpandoWrapper(object item)
        {
            _item = item;
            var itemType = item.GetType();
            foreach (var propertyInfo in itemType.GetProperties())
            {
                _ignoreCaseLookup.Add(propertyInfo.Name, propertyInfo);
                AddBox(propertyInfo.Name, propertyInfo.GetValue(_item));
            }
        }

        public IEnumerable<KeyValuePair<string, object>> Properties => _ignoreCaseLookupExtra.ToList()
            .Select(x => new KeyValuePair<string, object>(x.Key, x.Value.Item));

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            PropertyInfo lookup;
            var lowerInvariant = binder.Name.ToLowerInvariant();
            _ignoreCaseLookup.TryGetValue(lowerInvariant, out lookup);

            if (lookup != null)
            {
                result = lookup.GetValue(_item);
                return true;
            }

            Box box;
            _ignoreCaseLookupExtra.TryGetValue(lowerInvariant, out box);

            if (box != null)
            {
                result = box.Item;
                return true;
            }

            return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            PropertyInfo lookup;
            var propertyName = binder.Name.ToLowerInvariant();
            _ignoreCaseLookup.TryGetValue(propertyName, out lookup);

            if (lookup != null)
            {
                lookup.SetValue(_item, value);
                return true;
            }

            var box = new Box(value);
            _ignoreCaseLookupExtra[propertyName] = box;

            return true;
        }
    }
}
