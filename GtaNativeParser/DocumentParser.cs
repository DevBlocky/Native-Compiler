using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using HtmlAgilityPack;

namespace GtaNativeParser
{
    public static class DocumentParser
    {
        public static Native[] ExtremeParse(HtmlDocument doc)
        {
            IList<HtmlNode> nativeNodes =
                doc.DocumentNode.Descendants().Where(item => item.HasClass("native")).ToList();
            Console.WriteLine($"{nativeNodes.Count} natives found!");

            Console.WriteLine("Turning all of the html nodes into natives... might take a min");
            IList<Native> natives = new List<Native>();
            foreach (var item in nativeNodes)
            {
                // finding native nodes
                string txt = GetNodeText(item);

                // finding the native items
                string _namespace = item.GetAttributeValue("data-ns", "UNKNOWN");
                string name = item.GetAttributeValue("data-native", "UNKNOWN");
                if (name.StartsWith("0"))
                    name = "_" + name; // for native that don't have a name
                ulong hash = ulong.Parse(new string(item.GetAttributeValue("id", "_0x0").Skip(3).ToArray()),
                    NumberStyles.HexNumber);
                string _return = GetStringReturnType(txt);
                NativeType type = GetNodeNativeType(item);
                Parameter[] _params = GetStringParameters(txt);

                natives.Add(new Native(name, _namespace, hash, _return, _params, type));
            }
            Console.WriteLine("Woah! That didn't take very long! Next step...");

            return natives.ToArray();
        }

        private static string GetNodeText(HtmlNode item)
        {
            HtmlNode _in = item.Descendants().First(x => x.HasClass("c"));

            HtmlNode[] items = _in.ChildNodes.Where(x => !x.InnerText.StartsWith("//")).ToArray();
            string text = items.Aggregate(string.Empty,
                (current, node) => current + node.InnerText.Replace("&#40;", "(").Replace("&#41;", ")"));

            return text.Split('\n').Skip(1).Aggregate(string.Empty, (current, ln) => current + ln);
        }

        private static Parameter[] GetStringParameters(string input)
        {
            int open = input.IndexOf('(') + 1;
            int close = input.IndexOf(')') - open;

            string[] txt = new string(input.Skip(open).Take(close).ToArray()).Replace(", ", ",").ReplaceCppTypes()
                .Split(',');

            List<Parameter> param = txt.Select(item => item.Split(' ')).TakeWhile(item2 => item2.Length != 1)
                .Select(item2 => new Parameter(item2[1], item2[0])).ToList();

            return param.ToArray();
        }

        private static string GetStringReturnType(string input)
        {
            return input.Split(' ')[0].ReplaceCppTypes();
        }

        private static NativeType GetNodeNativeType(HtmlNode item)
        {
            if (item.GetAttributeValue("data-ns", "UNKNOWN") != "CFX") return NativeType.Client;

            var node = item.Descendants("p").First(x => x.InnerText.StartsWith("CitizenFX API set: "));
            var txt = node.InnerText.Replace("CitizenFX API set: ", "");
            return txt == "Server" ? NativeType.Server : txt == "Client" ? NativeType.Client : NativeType.Shared;
        }

        private static string ReplaceCppTypes(this string input)
        {
            return input.Replace("string", "str").Replace("char*", "string").Replace("BOOL", "bool").Replace("*", "")
                .Replace("Hash", "long").Replace("Any", "dynamic").Replace("class", "_class").Replace("event", "_event")
                .Replace("default", "_default").Replace("object", "_object").Replace("private", "_private")
                .Replace("out", "_out").Replace("base", "_base").Replace("ScrHandle", "int").Replace("Cam", "int")
                .Replace("Object", "int").Replace("func", "dynamic");
        }
    }
}
