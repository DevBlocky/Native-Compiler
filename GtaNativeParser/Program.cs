using System;
using System.Linq;
using System.Threading;
using HtmlAgilityPack;

namespace GtaNativeParser
{
    static class Program
    {
        internal const string REF_NATIVE_WEB = "https://runtime.fivem.net/doc/reference.html";
        internal const string CLIENT_SAVE_PATH = "ClientNatives.cs";
        internal const string SERVER_SAVE_PATH = "ServerNatives.cs";

        private static HtmlDocument doc;
        private static readonly HtmlWeb web = new HtmlWeb();

        public static void Main(string[] args)
        {
            Console.WriteLine("Gimme a min to read the web site...");
            Console.WriteLine("NOTE: This may take up a lot of ram :)");
            Console.WriteLine();
            DateTime start = DateTime.Now;
            doc = web.Load(REF_NATIVE_WEB);
            Console.WriteLine($"Woofff... That took {(int)(DateTime.Now - start).TotalMilliseconds} milliseconds to download. Let's get into the juicy part");

            if (doc.ParseErrors.Any())
            {
                Console.WriteLine("Damb, looks like we ran into an error, plz restart...");
                return;
            }
            Console.WriteLine();
            Console.WriteLine("Time to start parsing the web stuff, plz wait for a sec");
            Native[] natives = DocumentParser.ExtremeParse(doc);
            Console.WriteLine(
                $"Found {natives.Length} natives, " +
                $"of which {natives.Count(x => x.Type == NativeType.Client || x.Type == NativeType.Shared)} are usable");
            Console.WriteLine();

            Console.WriteLine("Gonna delete some things, let's hope this clears some RAM");
            doc = null;
            Console.WriteLine();

            Console.WriteLine("With all of this parsed, we can start with the saving part!");
            Console.WriteLine("Processing the Client natives");
            new NativeWriter(CLIENT_SAVE_PATH, natives.Where(x => x.Type == NativeType.Client || x.Type == NativeType.Shared).ToArray()).Run();
            Console.WriteLine();
            Console.WriteLine("Processing the Server natives");
            new NativeWriter(SERVER_SAVE_PATH, natives.Where(x => x.Type == NativeType.Server || x.Type == NativeType.Shared).ToArray()).Run();
            Console.WriteLine();

#if DEBUG
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
#else
            Console.WriteLine("And it looks like we're done! This entire screen will close in about 5");
            Thread.Sleep(5000);
#endif
        }
    }
}
