using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestChInvestBank.Services
{
    public class ArchiveExtractor
    {
        public string Extract(string zipPath, string extractTo)
        {
            if (!Directory.Exists(extractTo))
                Directory.CreateDirectory(extractTo);

            Console.WriteLine($"> Распаковка архива {zipPath}");
            ZipFile.ExtractToDirectory(zipPath, extractTo, true);
            Console.WriteLine("> Распаковка завершена.");

            return extractTo;
        }
    }
}
