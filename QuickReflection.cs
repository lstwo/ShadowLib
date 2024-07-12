using HarmonyLib;
using System.Reflection;

namespace ShadowLib
{
    public class QuickReflection<T>
    {
        public T instance;
        public BindingFlags flags;

        public QuickReflection(T instance, BindingFlags flags)
        {
            this.instance = instance;
            this.flags = flags;
        }

        public object GetField(string field)
        {
            return GetField(instance, flags, field);
        }

        public object GetMethod(string method, params object[] args)
        {
            return GetMethod(instance, method, args);
        }

        public void SetField(string field, object value)
        {
            SetField(instance, flags, field, value);
        }


        public static object GetField(object instance, BindingFlags flags, string field)
        {
            var info = instance.GetType().GetField(field, flags);
            return info.GetValue(instance);
        }

        public static object GetMethod(object instance, string method, params object[] args)
        {
            var info = Traverse.Create(instance).Method(method, args);
            return info.GetValue(args);
        }

        public static void SetField(object instance, BindingFlags flags, string field, object value)
        {
            var info = instance.GetType().GetField(field, flags);
            info.SetValue(instance, value);
        }
    }
}