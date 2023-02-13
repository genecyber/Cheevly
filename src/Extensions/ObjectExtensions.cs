using System;
using System.ComponentModel;
using System.Reflection;

namespace Cheevly.Extensions {
    public static class ObjectExtensions {
        public static T GetFieldValue<T>(this object source, string fieldName) {
            return (T)source.GetType().FindField(fieldName).GetValue(source);
        }

        public static T GetPropertyValue<T>(this object source, string propertyName) {
            return (T)source.GetType().FindProperty(propertyName).GetValue(source, null);
        }

        public static object As<T>(this object source, Func<T, object> action) where T : class {
            var target = source.As<T>();

            if (target != null) {
                return action(target);
            }

            return default(T);
        }

        public static T As<T>(this object source) where T : class {
            if (source.GetType() == typeof(T))
                return (T)source;

            return source.As(typeof(T)) as T;
        }

        public static object As(this object source, Type type) {
            return new TypeConverter().ConvertTo(source, type);
        }

        public static object InvokeStaticGenericMethod(this Type type, string methodName, Type genericType, params object[] parameters) {
            var methods = type.GetMethods(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            var method = methods.First(each => each.Name == methodName && each.IsGenericMethod);
            var genericMethod = method.MakeGenericMethod(genericType);

            return genericMethod.Invoke(null, parameters);
        }

        public static object InvokeStaticGenericMethod(this Type type, string methodName, Type genericType1, Type genericType2, params object[] parameters) {
            var methods = type.GetMethods(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            var method = methods.First(each => each.Name == methodName && each.IsGenericMethod);
            var genericMethod = method.MakeGenericMethod(genericType1, genericType2);

            return genericMethod.Invoke(null, parameters);
        }

        public static object InvokeGenericMethod<T>(this T source, string methodName, Type genericType, params object[] parameters) {
            var type = source.GetType();
            var genericMethod = type.GetGenericMethod(methodName, genericType);
            return genericMethod.Invoke(source, parameters);
        }
    }
}