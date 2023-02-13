using System;
using System.Reflection;
using Cheevly.Runtime.Exceptions;
using Cheevly.Skills;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Cheevly.CodeExecution {
    public class CSharpExecutionContext {

        private const string GeneratedNamespace = "DynamicCodeGeneration";
        private const string GeneratedClassName = "Runtime";
        private const string GeneratedMethodName = "Run";

        private readonly List<string> _imports = new() { "using System;", "using System.Runtime;" };

        public CSharpExecutionContext(params string[] imports) {

            foreach (var import in imports) {
                var formatted = "using " + import + ";";

                if (!_imports.Contains(formatted))
                    _imports.Add(formatted);
            }
        }

        public static CSharpExecutionContext WithTypeImports(params Type[] imports) {
            return new CSharpExecutionContext(imports.Select(each => each.Namespace).ToArray());
        }

        public object InvokeMethod(string methodCode, params MethodParameterBinding[] parameters) {

            try {
                return InternalInvokeMethod("return " + methodCode, "object", parameters);
            }
            catch (InvalidCompilationException) {
                return InternalInvokeMethod(methodCode, "void", parameters);
            }
        }

        private object InternalInvokeMethod(string methodCode, string returnType, params MethodParameterBinding[] parameters) {
            var compilation = Compile(methodCode, returnType, parameters);

            using var ms = new MemoryStream();

            var result = compilation.Emit(ms);

            if (!result.Success) {
                var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                foreach (var diagnostic in failures)
                    Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());

                if (failures.Any(each => each.Id == "CS0029"))
                    throw new InvalidCompilationException();

                throw new Exception(result.Diagnostics.First().ToString());
            }

            ms.Seek(0, SeekOrigin.Begin);
            var assembly = Assembly.Load(ms.ToArray());

            return InvokeGeneratedMethod(assembly, parameters);
        }

        private CSharpCompilation Compile(string methodCode, string returnType, params MethodParameterBinding[] parameters) {
            var methodSignature = string.Empty;

            foreach (var parameter in parameters) {
                if (!string.IsNullOrEmpty(methodSignature))
                    methodSignature += ", ";

                methodSignature += parameter.Value.GetType().Name + " " + parameter.Name;
            }

            var imports = string.Join("\n", _imports);

            var template =
@$"{imports}

namespace {GeneratedNamespace}
{{
    public class {GeneratedClassName}
    {{
        public static {returnType} {GeneratedMethodName}({methodSignature}) {{
            {methodCode}
        }}
    }}
}}";
            var syntaxTree = CSharpSyntaxTree.ParseText(template);
            var dotNetCoreDir = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);

            return CSharpCompilation
                .Create(Path.GetRandomFileName(), new[] { syntaxTree })
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(
                    MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Console).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(TimeSkills).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(Path.Combine(dotNetCoreDir, "System.Runtime.dll"))
                );
        }

        private object InvokeGeneratedMethod(Assembly assembly, params MethodParameterBinding[] parameters) {
            var type = assembly.GetType($"{GeneratedNamespace}.{GeneratedClassName}");
            var obj = Activator.CreateInstance(type);

            return type.InvokeMember(GeneratedMethodName, BindingFlags.Default | BindingFlags.InvokeMethod, null, obj, parameters.Select(each => each.Value).ToArray());
        }
    }
}