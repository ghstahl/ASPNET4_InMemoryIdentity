using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace P5.IdentityServerCore.Extensions
{
    public static class TypeExtensions
    {
        public static IEnumerable<FieldInfo> GetConstants(this Type type)
        {
            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            return fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly);
        }
        public static IEnumerable<FieldInfo> GetConstants<T>(this Type type)
        {
            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            var query = from fi in fieldInfos
                        where fi.FieldType == typeof(T) && fi.IsLiteral && !fi.IsInitOnly
                        select fi;

            return query;
        }
        public static IEnumerable<T> GetConstantsValues<T>(this IEnumerable<FieldInfo> fieldInfos) where T : class
        {
            var query = from fi in fieldInfos
                where fi.FieldType == typeof (T)
                select fi.GetRawConstantValue() as T;

            return query;
        }

        public static IEnumerable<T> GetConstantsValues<T>(this Type type) where T : class
        {
            var fieldInfos = GetConstants(type);

            return fieldInfos.Select(fi => fi.GetRawConstantValue() as T);
        }
    }
}
