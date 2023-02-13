using System.Collections;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cheevly.Extensions {
    public static class TypeExtensions {

        public static async Task<string> GetSourceCodeAsync(this Type type) {
            var path = Path.GetDirectoryName(type.Assembly.Location);
            var sln = Directory.GetParent(path).Parent.Parent;
            var folder = type.Namespace.Replace(sln.Name + ".", "");
            var file = Path.Combine(sln.ToString(), folder, type.Name + ".cs");
            var code = await File.ReadAllTextAsync(file);
            var tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetCompilationUnitRoot();
            var namespaceSyntax = root.Members.OfType<NamespaceDeclarationSyntax>().First();
            var implementation = namespaceSyntax.Members.OfType<ClassDeclarationSyntax>().First().ToFullString();
            return implementation;
        }

        public static T GetAttribute<T>(this Type type) where T : Attribute {
            return (T)type.GetCustomAttribute(typeof(T));
        }

        public static void WithAttribute<T>(this Type type, Action<T> handler) where T : Attribute {
            var attribute = type.GetAttribute<T>();

            if (attribute != null) {
                handler(attribute);
            }
        }

        public static MethodInfo GetGenericMethod(this Type type, string methodName, Type genericType) {
            var methods = type.GetMethods(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            var method = methods.First(each => each.Name == methodName && each.IsGenericMethod);
            return method.MakeGenericMethod(genericType);
        }

        public static object InvokeExtensionMethod(this Type type, string methodName, Type genericType, object target) {
            return type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(genericType).Invoke(null, new[] { target });
        }

        public static FieldInfo FindField(this Type type, string name) {
            return type.GetField(name, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static PropertyInfo FindProperty(this Type type, string name) {
            return type.GetProperty(name, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static MemberInfo FindMember(this Type type, string name) {
            return type.GetMember(name, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault();
        }

        public static bool IsStatic(this Type type) {
            return type.IsAbstract && type.IsSealed;
        }

        public static bool HasInterface(this Type type, Type interfaceType) {
            if (interfaceType.IsGenericType) {
                return type.GetInterfaces().Any(each => each.IsGenericType && each.GetGenericTypeDefinition() == interfaceType);
            }

            return interfaceType.IsAssignableFrom(type);
        }

        public static bool HasInterface<T>(this Type type) {
            return type.GetInterfaces().Any(each => each == typeof(T));
        }

        public static PropertyInfo GetPropertyWithAttribute<T>(this Type type) where T : Attribute {
            foreach (var property in type.GetProperties()) {
                if (property.HasAttribute<T>()) {
                    return property;
                }
            }

            return null;
        }

        public static bool IsSimple(this Type type) {
            if (!type.IsEnum && !type.FullName.StartsWith("System."))
                return false;

            if (type.IsGenericType && type.GetEnumerableType() != null)
                return false;

            return true;
        }

        public static List<MethodInfo> GetDeclaredMethods(this Type type, bool includeStatic = false, bool includePrivate = false) {

            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;

            if (includeStatic) {
                flags = flags | BindingFlags.Static;
            }

            if (includePrivate) {
                flags = flags | BindingFlags.NonPublic;
            }

            return type.GetMethods(flags).Where(m => !m.IsSpecialName).ToList();
        }

        public static IEnumerable<Type> GetGenericInterfacesOfType<T>(this Type type) {
            return type.GetInterfacesOfType<T>().Where(each => each.IsGenericType);
        }

        public static IEnumerable<Type> GetInterfacesOfType<T>(this Type type) {
            return type.GetBaseTypes().Where(each => each.IsInterface && typeof(T).IsAssignableFrom(each));
        }

        public static List<Type> GetBaseTypes(this Type type) {
            var types = type.GetInterfaces().ToList();

            if (type.BaseType != null && !types.Contains(type.BaseType))
                types.Add(type.BaseType);

            foreach (var eachBaseType in types.ToList()) {
                var parentTypes = eachBaseType.GetBaseTypes();

                foreach (var parentType in parentTypes)
                    if (!types.Contains(parentType))
                        types.Add(parentType);
            }

            return types;
        }

        public static object New(this Type type) {
            return Activator.CreateInstance(type);
        }

        public static T New<T>(this Type type, params Type[] genericTypes) {
            if (genericTypes.Any()) {
                type = type.MakeGenericType(genericTypes);
            }

            return (T)Activator.CreateInstance(type);
        }

        public static Type GetEnumerableType(this Type type) {
            return (from intType in type.GetInterfaces()
                    where intType.IsGenericType && intType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                    select intType.GetGenericArguments()[0]).FirstOrDefault();
        }

        public static IList MakeList(this Type type) {
            var genericListType = typeof(List<>).MakeGenericType(type);
            return (IList)Activator.CreateInstance(genericListType);
        }

        public static bool Is<T>(this Type type) {
            return typeof(T).IsAssignableFrom(type);
        }

        public static bool IsCollection(this Type type) {
            if (type == typeof(string)) {
                return false;
            }

            return type.Is<IList>() || type.Is<ICollection>() || type.Is<IEnumerable>();
        }

        public static bool IsNullableType(this Type type) {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsNonSystemClass(this Type type) {
            return type.Namespace != "System" && !type.IsPrimitive;
        }

        public static bool IsComplexType(this Type type) {
            return type.IsClass && !type.FullName.StartsWith("System");
        }

        public static List<Type> GetTypesOf(this Type type, List<Assembly> assemblies = null, bool includeAbstract = false) {
            if (assemblies == null) {
                assemblies = new List<Assembly> { Assembly.GetAssembly(type) };
            }

            return assemblies.SelectMany(assembly => assembly.GetTypes())
                .Where(each => type.IsAssignableFrom(each) && (includeAbstract || each.IsAbstract == false))
                .ToList();
        }

        public static IEnumerable<Tuple<MethodInfo, T>> GetMethodsWithAttribute<T>(this Type type) where T : Attribute {
            foreach (var method in type.GetMethods()) {
                var attribute = method.GetCustomAttribute<T>(true);

                if (attribute != null) {
                    yield return new Tuple<MethodInfo, T>(method, attribute);
                }
            }
        }

        public static object Default(this Type type) {
            return typeof(TypeExtensions).InvokeStaticGenericMethod(nameof(Default), type);
        }

        public static object Default<T>() {
            return default(T);
        }
    }
}