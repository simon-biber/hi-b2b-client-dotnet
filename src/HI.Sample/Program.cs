using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nehta.VendorLibrary.HI.Sample
{
    class Program
    {
        public static void Main()
        {
            var s = new ProviderReadReferenceDataClientSample();
            var r = s.Sample();
            if (r.Item1 != null)
            {
                Console.WriteLine("Result:");
                Console.WriteLine("[");
                foreach (var item in r.Item1.readReferenceDataResult.elementReferenceValues)
                {
                    Console.WriteLine("    {");
                    Console.WriteLine($"    \"name\": \"{item.elementName}\"");
                    Console.WriteLine("    \"referenceSet\", [");
                    foreach (var i in item.referenceSet)
                    {
                        Console.WriteLine($"        \"{i.referenceCode}\": \"{i.referenceDescription}\",");
                    }
                    Console.WriteLine("    ]},");
                }
                Console.WriteLine("]");
                Console.WriteLine();
            }
            if (r.Item3 != null)
            {
                Console.WriteLine("Response:");
                Console.WriteLine(r.Item3.ToString());
                Console.WriteLine();
            }
            if (r.Item2 != null)
            {
                Console.WriteLine("Exception:");
                Console.WriteLine(r.Item2.ToString());
                Console.WriteLine();
            }
            Console.WriteLine("Press ENTER to end");
            Console.ReadLine();
        }
    }
}
