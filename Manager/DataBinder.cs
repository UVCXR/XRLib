using System;
using System.Collections.Generic;
using System.Reflection;

namespace WIFramework
{
    public static partial class DataBinder
    {
        static Dictionary<string, Model> models = new Dictionary<string, Model>();
        public partial class Model
        {
            object data;
            private FieldInfo[] fields;
            private PropertyInfo[] properties;

            Dictionary<string, FieldInfo> fieldTable;
            Dictionary<FieldInfo, object> fieldDataTable;
            Dictionary<FieldInfo, Action> fieldDataChanged;

            Dictionary<string, PropertyInfo> propertyTable;
            Dictionary<PropertyInfo, object> propertyDataTable;
            Dictionary<PropertyInfo, Action> propertyDataChanged;
            public Model(object data)
            {
                this.data = data;
                fields = data.GetType().GetAllFields();
                fieldTable = new Dictionary<string, FieldInfo>();
                fieldDataTable = new Dictionary<FieldInfo, object>();
                fieldDataChanged = new Dictionary<FieldInfo, Action>();
                foreach (var f in fields)
                {
                    fieldTable.Add(f.Name, f);
                    fieldDataTable.Add(f, f.GetValue(data));
                }

                properties = data.GetType().GetAllProperties();
                propertyTable = new Dictionary<string, PropertyInfo>();
                propertyDataTable = new Dictionary<PropertyInfo, object>();
                propertyDataChanged = new Dictionary<PropertyInfo, Action>();
                foreach (var p in properties)
                {
                    if (!p.CanRead || !p.CanWrite)
                        continue;
                    propertyTable.Add(p.Name, p);
                    propertyDataTable.Add(p, p.GetValue(data));
                }
            }

            public object GetPropertyValue(string name)
            {
                if (propertyTable.TryGetValue(name, out var value))
                {
                    return value.GetValue(data);
                }

                return null;
            }

            public object GetFieldValue(string name)
            {
                if (fieldTable.TryGetValue(name, out var value))
                {
                    return value.GetValue(data);
                }

                return null;
            }
            public bool FieldValueChangeCheck(string name)
            {
                if (!fieldTable.TryGetValue(name, out var value))
                {
                    return false;
                }

                var prev = fieldDataTable[value];
                var curr = value.GetValue(data);
                if (curr == null)
                    return false;
                if (prev != null && prev.Equals(curr))
                {
                    return false;
                }
                if (fieldDataChanged.TryGetValue(value, out var action))
                {
                    action?.Invoke();
                }
                fieldDataTable[value] = curr;
                return true;
            }

            public bool PropertyValueChangeCheck(string name)
            {
                if (!propertyTable.TryGetValue(name, out var value))
                    return false;

                var prev = propertyDataTable[value];
                var curr = value.GetValue(data);

                if (prev.Equals(curr))
                {
                    return false;
                }

                if (propertyDataChanged.TryGetValue(value, out var action))
                {
                    action?.Invoke();
                }
                return true;
            }

            public bool SetFieldEvent(string variableName, Action action)
            {
                if (!fieldTable.TryGetValue(variableName, out var fieldInfo))
                {
                    return false;
                }

                if (!fieldDataChanged.TryGetValue(fieldInfo, out var value))
                {
                    fieldDataChanged.Add(fieldInfo, action);
                }
                else
                    fieldDataChanged[fieldInfo] += action;
                return true;
            }

            public bool SetPropertyEvent(string propertyName, Action action)
            {
                if (!propertyTable.TryGetValue(propertyName, out var propertyInfo))
                {
                    return false;
                }

                if (!propertyDataChanged.TryGetValue(propertyInfo, out var value))
                {
                    propertyDataChanged.Add(propertyInfo, action);
                }
                else
                {
                    propertyDataChanged[propertyInfo] += action;
                }
                return true;
            }
        }

        public static Model SetModel(object target)
        {
            var model = new Model(target);
            return model;
        }

        public static Model Binding(this Model model, string targetVariable, System.Action action, bool readyToAction = false)
        {
            if (model.SetFieldEvent(targetVariable, action))
            {
                if (readyToAction)
                    action.Invoke();
            }
            else if (model.SetPropertyEvent(targetVariable, action))
            {
                if (readyToAction)
                    action.Invoke();
            }
            else
                return model;

            models.Add(targetVariable, model);
            return model;
        }

        internal static void Update()
        {
            foreach (var m in models)
            {
                if (m.Value.FieldValueChangeCheck(m.Key))
                    continue;

                m.Value.PropertyValueChangeCheck(m.Key);
            }
        }
    }

}