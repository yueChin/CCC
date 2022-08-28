using System;
using System.Reflection;

namespace Kits.DevlpKit.Helpers.ReflectionHelper
{
    public static class MemberInfoHelper
    {
        public static object GetMemberValue<T>(string memberName)
        {
            object value = null;
            Type type = typeof(T);
            return GetMemberValue(type, memberName);
        }

        public static object GetMemberValue(string typeName, string memberName)
        {
            Type type = TypeInfoHelper.GetType(typeName);
            return GetMemberValue(type, memberName);
        }

        public static object GetMemberValue(Type type, string memberName, object instance = null)
        {
            object value = null;
            if (type != null || instance != null)
            {
                BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic;
                if (type == null)
                {
                    type = instance.GetType();
                    flags |= BindingFlags.Instance;
                }
                else
                {
                    instance = null;
                    flags |= BindingFlags.Static;
                }

                MemberInfo[] members = type.GetMembers(flags);
                for (int i = 0, count = members.Length; i < count && value == null; ++i)
                {
                    MemberInfo m = members[i];
                    if ((m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property) &&
                        m.Name == memberName)
                    {
                        PropertyInfo pi = m as PropertyInfo;
                        if (pi != null)
                        {
                            value = pi.GetValue(instance, null);
                        }
                        else
                        {
                            FieldInfo fi = m as FieldInfo;
                            if (fi != null)
                            {
                                value = fi.GetValue(instance);
                            }
                        }
                    }
                }
            }

            return value;
        }

        public static bool SetMemberValue(string typeName, string memberName, object value)
        {
            Type type = TypeInfoHelper.GetType(typeName);
            return SetMemberValue(type, memberName, value);
        }

        public static bool SetMemberValue(Type type, string memberName, object value, object instance = null)
        {
            if (type != null || instance != null)
            {
                BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic;
                if (type == null)
                {
                    type = instance.GetType();
                    flags |= BindingFlags.Instance;
                }
                else
                {
                    instance = null;
                    flags |= BindingFlags.Static;
                }

                MemberInfo[] members = type.GetMembers(flags);
                for (int i = 0; i < members.Length; ++i)
                {
                    MemberInfo m = members[i];
                    if ((m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property) &&
                        m.Name == memberName)
                    {
                        PropertyInfo pi = m as PropertyInfo;
                        if (pi != null)
                        {
                            pi.SetValue(instance, value, null);
                            return true;
                        }
                        else
                        {
                            FieldInfo fi = m as FieldInfo;
                            if (fi != null && fi.IsLiteral == false && fi.IsInitOnly == false)
                            {
                                fi.SetValue(instance, value);
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}