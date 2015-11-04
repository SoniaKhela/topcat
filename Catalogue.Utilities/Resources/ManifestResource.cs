using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Catalogue.Utilities.Resources
{
    public static class ManifestResource
    {
        public static Stream Open(Assembly assembly, string name)
        {
            string fullName = assembly.GetName().Name + "." + name;
            return assembly.GetManifestResourceStream(fullName);
        }

        public static IEnumerable<string> ReadLines(Assembly assembly, string name)
        {
            using (var stream = Open(assembly, name))
            {
                var r = new StreamReader(stream);

                while (!r.EndOfStream)
                    yield return r.ReadLine();
            }
        }
    }
}
