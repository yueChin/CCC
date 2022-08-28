using System;
using System.Reflection;

namespace Kits.DevlpKit.Helpers.ReflectionHelper
{
	public static class PropertyInfoHelper
	{
        /// <summary>
        /// 获取对象的成员属性信息。从最终类型开始向上查找，忽略属性的可见性。
        /// </summary>
        public static PropertyInfo GetPropertyInfo(object instance, string propertyName)
        {
            Type type = instance.GetType();
            PropertyInfo info = null;
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            while (type != null)
            {
                info = type.GetProperty(propertyName, flags);
                if (info != null) return info;
                type = type.BaseType;
            }

            return null;
        }


        /// <summary>
        /// 获取类型的静态属性信息。从最终类型开始向上查找，忽略属性的可见性。
        /// </summary>
        public static PropertyInfo GetPropertyInfo<T>(string propertyName)
        {
            Type type = typeof(T);
            PropertyInfo info = null;
            BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

            while (type != null)
            {
                info = type.GetProperty(propertyName, flags);
                if (info != null) return info;
                type = type.BaseType;
            }

            return null;
        }

        /// <summary>
        /// 获取对象的成员属性值。从最终类型开始向上查找，忽略属性的可见性。
        /// </summary>
        public static object GetPropertyValue(object instance, string propertyName)
        {
            return GetPropertyInfo(instance, propertyName).GetValue(instance, null);
        }


        /// <summary>
        /// 获取类型的静态属性值。从最终类型开始向上查找，忽略属性的可见性。
        /// </summary>
        public static object GetPropertyValue<T>(string propertyName)
        {
            return GetPropertyInfo<T>(propertyName).GetValue(null, null);
        }


        /// <summary>
        /// 设置对象的成员属性值。从最终类型开始向上查找，忽略属性的可见性。
        /// </summary>
        public static void SetPropertyValue(object instance, string propertyName, object value)
        {
            GetPropertyInfo(instance, propertyName).SetValue(instance, value, null);
        }


        /// <summary>
        /// 设置类型的静态属性值。从最终类型开始向上查找，忽略属性的可见性。
        /// </summary>
        public static void SetPropertyValue<T>(string propertyName, object value)
        {
            GetPropertyInfo<T>(propertyName).SetValue(null, value, null);
        }
}
}