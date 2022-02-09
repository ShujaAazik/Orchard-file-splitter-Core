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
            try
            {
                IHost host = Host.CreateDefaultBuilder(args).Build();
                IConfiguration config = host.Services.GetRequiredService<IConfiguration>();
                FileSplitter fileSplitter = new FileSplitter(config);
                fileSplitter.SplitFile();
                Console.ReadKey();
                host.RunAsync();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
