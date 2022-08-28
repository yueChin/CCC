using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kits.DevlpKit.Helpers.ReflectionHelper
{
    public class AttributesHelper
    {
        /// <summary>
        /// 获得类字段指定标识列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<T> GetPropertyAttributes<T>(object obj) where T : Attribute
        {
            List<T> list = new List<T>();
            FindAllAttribute(list, obj);
            return list;
        }

        /// <summary>
        /// 查找所有的指定Attributes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="obj"></param>
        public static void FindAllAttribute<T>(List<T> list, object obj) where T : Attribute
        {
            Type objType = obj.GetType();
            foreach (PropertyInfo propertyInfo in objType.GetProperties())
            {
                Attribute attribute = Attribute.GetCustomAttribute(propertyInfo, typeof(T));
                if (attribute == null) continue;

                Type proType = propertyInfo.GetType();
                if (proType.IsEnum || proType.IsValueType || proType == typeof(String))
                    list.Add(attribute as T);
                else
                {
                    FindAllAttribute<T>(list, Activator.CreateInstance(proType));
                }
            }
        }
    }
}
