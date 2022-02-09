using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace Orchard_file_splitter_Core
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args).Build();
            IConfiguration config = host.Services.GetRequiredService<IConfiguration>();
            FileSplitter fileSplitter = new FileSplitter(config);
            Console.WriteLine("Splitting Process Started.\n");
            fileSplitter.SplitFile();
            Console.WriteLine("Splitting Process is Completed.");
            host.RunAsync();
        }
    }
}
