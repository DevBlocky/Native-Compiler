/*
 * This is just some basic storage vars to keep the code clean!
*/

namespace GtaNativeParser
{
    public enum NativeType
    {
        Shared,
        Server,
        Client
    }
    public class Native
    {
        public string Namespace { get; }
        public string Name { get; }
        public Parameter[] Parameters { get; }
        public ulong Hash { get; }
        public string Return { get; }
        public NativeType Type { get; }

        public Native(string name, string _namespace, ulong hash, string returnType, Parameter[] _params, NativeType type)
        {
            Name = name;
            Namespace = _namespace;
            Hash = hash;
            Return = returnType;
            Parameters = _params;
            Type = type;
        }
    }

    public class Parameter
    {
        public string Type { get; }
        public string Name { get; }

        public Parameter(string name, string type)
        {
            Name = name;
            Type = type;
        }
    }
}
