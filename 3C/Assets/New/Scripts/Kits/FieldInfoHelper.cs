using System;
using System.Reflection;

namespace Kits.DevlpKit.Helpers.ReflectionHelper
{
    public static class FieldInfoHelper
    {
        /// 获取对象的成员字段信息。从最终类型开始向上查找，忽略字段的可见性。
        /// </summary>
        public static FieldInfo GetFieldInfo(object instance, string fieldName)
        {
            Type type = instance.GetType();
            FieldInfo info = null;
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            while (type != null)
            {
                info = type.GetField(fieldName, flags);
                if (info != null) 
                    return info;
                type = type.BaseType;
            }

            return null;
        }


        /// <summary>
        /// 获取类型的静态字段信息。从最终类型开始向上查找，忽略字段的可见性。
        /// </summary>
        public static FieldInfo GetFieldInfo<T>(string fieldName)
        {
            Type type = typeof(T);
            FieldInfo info = null;
            BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

            while (type != null)
            {
                info = type.GetField(fieldName, flags);
                if (info != null)
                    return info;
                type = type.BaseType;
            }

            return null;
        }

        /// <summary>
        /// 获取对象的成员字段值。从最终类型开始向上查找，忽略字段的可见性。
        /// </summary>
        public static object GetFieldValue(object instance, string fieldName)
        {
            return GetFieldInfo(instance, fieldName).GetValue(instance);
        }


        /// <summary>
        /// 获取类型的静态字段值。从最终类型开始向上查找，忽略字段的可见性。
        /// </summary>
        public static object GetFieldValue<T>(string fieldName)
        {
            return GetFieldInfo<T>(fieldName).GetValue(null);
        }

        /// <summary>
        /// 设置对象的成员字段值。从最终类型开始向上查找，忽略字段的可见性。
        /// </summary>
        public static void SetFieldValue(object instance, string fieldName, object value)
        {
            GetFieldInfo(instance, fieldName).SetValue(instance, value);
        }


        /// <summary>
        /// 设置类型的静态字段值。从最终类型开始向上查找，忽略字段的可见性。
        /// </summary>
        public static void SetFieldValue<T>(string fieldName, object value)
        {
            GetFieldInfo<T>(fieldName).SetValue(null, value);
        }
    }
}