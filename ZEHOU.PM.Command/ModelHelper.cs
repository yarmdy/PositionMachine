using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ZEHOU.PM.Command
{
    public static class ModelHelper
    {
        public static void CopyFrom<T1, T2>(this T1 obj,T2 obj2) where T1 : class,new () where T2 : class,new ()
        {
            if (obj2 == null)
            {
                return;
            }
            var t2props = typeof(T2).GetProperties(BindingFlags.Public|BindingFlags.GetProperty|BindingFlags.IgnoreCase|BindingFlags.Instance).ToDictionary(a=>a.Name.ToLower());
            if (t2props.Count <= 0) {
                return;
            }
            var t1props = typeof(T1).GetProperties(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.IgnoreCase | BindingFlags.Instance);
            foreach (var prop in t1props)
            {
                try
                {
                    var name = prop.Name.ToLower();
                    if (!t2props.ContainsKey(name))
                    {
                        continue;
                    }
                    var prop2 = t2props[name];
                    var p1type = prop.PropertyType;
                    var p2type = prop2.PropertyType;

                    var p2v = prop2.GetValue(obj2);
                    if (p1type == p2type)
                    {
                        prop.SetValue(obj, p2v);
                        continue;
                    }
                    bool p1null = false;
                    bool p2null = false;
                    if (p1type.FullName.StartsWith("System.Nullable") && p1type.GenericTypeArguments != null && p1type.GenericTypeArguments.Length > 0)
                    {
                        p1type = p1type.GenericTypeArguments[0];
                        p1null = true;
                    }
                    if (p2type.FullName.StartsWith("System.Nullable") && p2type.GenericTypeArguments != null && p2type.GenericTypeArguments.Length > 0)
                    {
                        p2type = p2type.GenericTypeArguments[0];
                        p2null = true;
                    }
                    if (p1type != p2type)
                    {
                        continue;
                    }
                    if (!p1null && p2null && p2v == null)
                    {
                        p2v = Activator.CreateInstance(p2type);
                    }
                    prop.SetValue(obj, p2v);
                }
                catch (Exception ex) {
                    continue;
                }
                
            }
        } 
    }
}
