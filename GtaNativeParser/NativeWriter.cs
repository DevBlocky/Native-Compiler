using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GtaNativeParser
{
    public class NativeWriter
    {
        private const string MASTER_NAMESPACE = "Natives";
        private const string NATIVE_PREFIX = "public static";
        private const int INDENT = 4;

        private readonly string path;
        private readonly Native[] natives;
        public NativeWriter(string path, Native[] natives)
        {
            this.path = path;
            this.natives = natives;
        }

        public void Run()
        {
            var namespaces = GetExistingNamespaces().ToArray();

            List<string> lines = new List<string>();

            Console.WriteLine("Writing initial lines");
            // starting lines
            lines.AddRange(new[]
            {
                "/*",
                "",
                "  File compiled by Native Compiler",
                "",
                "  Native Compiler by BlockBa5her ;)",
                "",
                "*/",
                "",
                "using CitizenFX.Core;",
                "using CitizenFX.Core.Native;",
                "",
                $"namespace {MASTER_NAMESPACE}",
                "{",
            });

            Console.WriteLine("Writing all of the natives");
            // native lines
            foreach (var item in namespaces)
            {
                Native[] n = natives.Where(x => x.Namespace == item).ToArray();
                // adding namespace start
                lines.AddRange(new[]
                {
                    $"public static class {item}",
                    "{"
                });

                // adding every native
                foreach (var native in n)
                    Console.WriteLine(CalculateNativeLine(native));
                lines.AddRange(n.Select(CalculateNativeLine));

                // adding namespace end
                lines.AddRange(new[]
                {
                    "}"
                });
            }

            Console.WriteLine("Ending the write");
            // ending lines
            lines.AddRange(new[]
            {
                "}"
            });

            Console.WriteLine("Sending all of the lines to the file!");
            File.WriteAllLines(path, LineIndentation(lines.ToArray()));
        }

        public static string CalculateNativeLine(Native native)
        {
            string paramTxt1 = "";
            string paramTxt2 = "";
            string typeref = native.Return == "void" ? "" : $"<{native.Return}>";
            if (!native.Parameters.Any())
                return
                    $"{NATIVE_PREFIX} {native.Return} {native.Name}({paramTxt1}) => Function.Call{typeref}((Hash){"0x" + native.Hash.ToString("X")}{paramTxt2});";

            for (int i = 0; i < native.Parameters.Length; i++)
            {
                Parameter param = native.Parameters[i];
                paramTxt1 += i == 0 ? $"{param.Type} {param.Name}" : $", {param.Type} {param.Name}";

                paramTxt2 += $", {param.Name}";
            }
            return
                $"{NATIVE_PREFIX} {native.Return} {native.Name}({paramTxt1}) => Function.Call{typeref}((Hash){"0x" + native.Hash.ToString("X")}{paramTxt2});";
        }

        public IEnumerable<string> GetExistingNamespaces()
        {
            List<string> namespaces = new List<string>();
            foreach (var item in natives)
            {
                if (namespaces.Contains(item.Namespace))
                    continue;

                namespaces.Add(item.Namespace);
            }

            return namespaces.ToArray();
        }

        public static string[] LineIndentation(string[] input)
        {
            string[] output = new string[input.Length];

            int level = 0;
            for (int i = 0; i < input.Length; i++)
            {
                string line = input[i];

                int instances = line.Count(x => x == '}');
                level -= instances;

                output[i] = new string(Enumerable.Repeat(' ', level * INDENT).ToArray()) + line;

                instances = line.Count(x => x == '{');
                level += instances;
            }

            return output;
        }
    }
}
