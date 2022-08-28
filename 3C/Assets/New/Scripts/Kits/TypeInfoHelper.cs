using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kits.DevlpKit.Helpers.ReflectionHelper
{

    /// <summary>
    /// 程序集相关的实用函数。
    /// </summary>
    public static class TypeInfoHelper
    {
        private static readonly System.Reflection.Assembly[] s_Assemblies = null;

        private static readonly Dictionary<string, Type> s_CachedTypes =
            new Dictionary<string, Type>(StringComparer.Ordinal);

        static TypeInfoHelper()
        {
            s_Assemblies = AppDomain.CurrentDomain.GetAssemblies();
        }

        /// <summary>
        /// 获取已加载的程序集。
        /// </summary>
        /// <returns>已加载的程序集。</returns>
        public static System.Reflection.Assembly[] GetAssemblies()
        {
            return s_Assemblies;
        }

        /// <summary>
        /// 获取已加载的程序集中的所有类型。
        /// </summary>
        /// <returns>已加载的程序集中的所有类型。</returns>
        public static Type[] GetTypes()
        {
            List<Type> results = new List<Type>();
            foreach (System.Reflection.Assembly assembly in s_Assemblies)
            {
                results.AddRange(assembly.GetTypes());
            }

            return results.ToArray();
        }

        /// <summary>
        /// 获取已加载的程序集中的所有类型。
        /// </summary>
        /// <param name="results">已加载的程序集中的所有类型。</param>
        public static void GetTypes(List<Type> results)
        {
            if (results == null)
            {
                //throw new GameFrameworkException("Results is invalid.");
                return;
            }

            results.Clear();
            foreach (System.Reflection.Assembly assembly in s_Assemblies)
            {
                results.AddRange(assembly.GetTypes());
            }
        }

        /// <summary>
        /// 获取已加载的程序集中的指定类型。
        /// </summary>
        /// <param name="typeName">要获取的类型名。</param>
        /// <returns>已加载的程序集中的指定类型。</returns>
        public static Type GetType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                //throw new GameFrameworkException("Type name is invalid.");
                return null;
            }

            Type type = null;
            if (s_CachedTypes.TryGetValue(typeName, out type))
            {
                return type;
            }

            type = Type.GetType(typeName);
            if (type != null)
            {
                s_CachedTypes.Add(typeName, type);
                return type;
            }

            foreach (System.Reflection.Assembly assembly in s_Assemblies)
            {
                type = Type.GetType($"{typeName}, {assembly.FullName}");
                if (type != null)
                {
                    s_CachedTypes.Add(typeName, type);
                    return type;
                }
            }

            return null;
        }

        /// <summary>
        /// 反射获得同一类型的
        /// </summary>
        /// <typeparam name="T">反射标记类型</typeparam>
        /// <param name="classType">指定程序集的类</param>
        /// <returns></returns>
        public static void GetAllInstanceClass<T>(List<object> list, Type classType)
        {
            Assembly editorAssembly = Assembly.GetAssembly(classType);

            Type[] typeArr = editorAssembly.GetTypes();
            foreach (Type type in typeArr)
            {
                object[] customTypeArr = type.GetCustomAttributes(typeof(T), false);

                if (customTypeArr.Length > 0)
                {
                    list.Add(CreateInstance(type));
                }
            }
        }

        /// <summary>
        /// 深度遍历创建实例对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object CreateInstance(Type type)
        {
            object obj = Activator.CreateInstance(type);

            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                Type propertyType = propertyInfo.PropertyType;
                if (!propertyType.IsValueType && propertyType != typeof(System.String) && !propertyType.IsEnum)
                {
                    propertyInfo.SetValue(obj, CreateInstance(propertyType), null);
                }
            }

            return obj;
        }
    }
}