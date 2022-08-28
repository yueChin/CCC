using System;
using System.Reflection;

namespace Kits.DevlpKit.Helpers.ReflectionHelper
{
	public static class MethodInfoHelper
	{
        public static readonly object[] BoxedEmpty = new object[] { };

        /// <summary>
        /// 获取对象的成员方法信息。从最终类型开始向上查找，忽略方法的可见性。
        /// </summary>
        public static MethodInfo GetMethodInfo(object instance, string methodName)
        {
            Type type = instance.GetType();
            MethodInfo info = null;
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            while (type != null)
            {
                info = type.GetMethod(methodName, flags);
                if (info != null) return info;
                type = type.BaseType;
            }

            return null;
        }


        /// <summary>
        /// 获取类型的静态方法信息。从最终类型开始向上查找，忽略方法的可见性。
        /// </summary>
        public static MethodInfo GetMethodInfo<T>(string methodName)
        {
            Type type = typeof(T);
            MethodInfo info = null;
            BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

            while (type != null)
            {
                info = type.GetMethod(methodName, flags);
                if (info != null) 
                    return info;
                type = type.BaseType;
            }

            return null;
        }

        /// <summary>
        /// 调用对象的成员方法。从最终类型开始向上查找，忽略方法的可见性。
        /// </summary>
        public static object InvokeMethod(object instance, string methodName, params object[] param)
        {
            return GetMethodInfo(instance, methodName).Invoke(instance, param);
        }


        /// <summary>
        /// 调用类型的静态方法。从最终类型开始向上查找，忽略方法的可见性。
        /// </summary>
        public static object InvokeMethod<T>(string methodName, params object[] param)
        {
            return GetMethodInfo<T>(methodName).Invoke(null, param);
        }
        
        /// <summary>
        /// 调用类型的静态方法。从最终类型开始向上查找，忽略方法的可见性。
        /// </summary>
        public static object InvokeMethod( string typeName, string methodName, object[] parameters = null )
        {
            Type type = TypeInfoHelper.GetType( typeName );
            if ( type != null ) 
            {
                MethodInfo f = type.GetMethod( methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static );
                if ( f != null ) 
                {
                    object r = f.Invoke( null, parameters ?? BoxedEmpty );
                    return r;
                }
            }
            return null;
        }

        /// <summary>
        /// 调用类型的静态方法。从最终类型开始向上查找，忽略方法的可见性。
        /// </summary>
        public static object InvokeMethod( Type type, string methodName, object[] parameters = null ) 
        {
            if ( type != null ) 
            {
                MethodInfo f = type.GetMethod( methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static );
                if ( f != null ) 
                {
                    object r = f.Invoke( null, parameters ?? BoxedEmpty );
                    return r;
                }
            }
            return null;
        }

        public static void Run(this MethodInfo methodInfo, object obj, params object[] param)
        {
            if (methodInfo.IsStatic)
            {
                object[] p = new object[param.Length + 1];
                p[0] = obj;
                for (int i = 0; i < param.Length; ++i)
                {
                    p[i + 1] = param[i];
                }
                methodInfo.Invoke(null, p);
            }
            else
            {
                methodInfo.Invoke(obj, param);
            }
        }

	}
}