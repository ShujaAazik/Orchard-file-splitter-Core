using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Orchard_file_splitter_Core
{
    internal class FileSplitter
    {
        private readonly string WithContractsFolder;
        private readonly string WithoutContractsFolder;
        private readonly string ProcessedFileFolder;
        private readonly string ErrorFileFolder;
        private readonly string InputFilesFolderPath;
        private readonly string[] ContractNos;
        private readonly string QUOTE = "\"";

        public FileSplitter(IConfiguration configuration)
        {
            InputFilesFolderPath = configuration["InputFilesFolderPath"];
            FileHandling.ValidateDirectory(configuration["WithContractsFolder"]);
            this.WithContractsFolder = configuration["WithContractsFolder"];
            FileHandling.ValidateDirectory(configuration["WithoutContractsFolder"]);
            this.WithoutContractsFolder = configuration["WithoutContractsFolder"];
            FileHandling.ValidateDirectory(configuration["ProcessedFileFolder"]);
            this.ProcessedFileFolder = configuration["ProcessedFileFolder"];
            FileHandling.ValidateDirectory(configuration["ErrorFileFolder"]);
            this.ErrorFileFolder = configuration["ErrorFileFolder"];
            this.ContractNos = configuration["ContractNos"].Split(',');
        }

        public void SplitFile()
        {
            var errorFileList = new List<string>();

            foreach (var file in Directory.GetFiles(InputFilesFolderPath))
            {
                try
                {
                    var fileName = Path.GetFileName(file);
                    if (!fileName.ToLower().Contains("jd"))
                    {
                        FileHandling.CopyFile(file, Path.Combine(WithContractsFolder, fileName));
                        FileHandling.CopyFile(file, Path.Combine(WithoutContractsFolder, fileName));
                        FileHandling.MoveFile(file, Path.Combine(ProcessedFileFolder, fileName));
                        continue;
                    }

                    Split(file, ContractNos, ref errorFileList);
                    FileHandling.MoveFile(file, Path.Combine(ProcessedFileFolder, Path.GetFileName(file)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"1 Error has Occured in Splitting Process\n");
                    Console.WriteLine(ex.Message);
                    errorFileList.Add($"File Name: {Path.GetFileName(file)} \t Error: {ex.Message} \t Time Occured: {DateTime.Now}");
                }
            }
            WriteToDirectory(Path.Combine(ErrorFileFolder, "Error File List"), errorFileList, "error");
        }

        private void Split(string file, string[] contractNos, ref List<string> errorFileList)
        {
            var withContract = new List<string>();
            var withoutContract = new List<string>();
            var fileName = Path.GetFileName(file);
            Console.WriteLine($"{File.ReadAllLines(file).Count()} line were found in the {Path.GetFileName(file)} file.\n");

            int index = 0;
            foreach (var line in File.ReadAllLines(file))
            {
                try
                {
                    var row = line.Split(new char[] { ' ' });
                    row = RefineColumns(row);
                    if (contractNos.Any(contactNo => contactNo.Equals(row[34])))
                    {
                        withContract.Add(line);
                        Console.WriteLine($"1 line has been identified as with Contract\n");
                    }
                    else
                    {
                        withoutContract.Add(line);
                        Console.WriteLine($"1 line has been identified as without Contract\n");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"1 line has been identified as Error\n");
                    Console.WriteLine(ex.Message);
                    errorFileList.Add($"File Name: {Path.GetFileName(file)} \t Error: {ex.Message} \t Line No: {index} \t Time Occured: {DateTime.Now}");
                }
                index++;
            }

            WriteToDirectory(Path.Combine(WithContractsFolder, Path.GetFileName(file)), withContract, "with contract");
            WriteToDirectory(Path.Combine(WithoutContractsFolder, Path.GetFileName(file)), withoutContract, "without contract");
        }

        private static void WriteToDirectory(string destinationDirectoryPath, List<string> fileList, string fileType)
        {
            if (fileList.Any())
            {
                Console.WriteLine($"{fileList.Count} number of line(s) were found as {fileType}.\n");
                FileHandling.WriteFile(destinationDirectoryPath, fileList);
            }
        }

        private string[] RefineColumns(string[] rows)
        {
            var refinedRow = new List<string>();
            var data = new StringBuilder();
            var activeRowSplit = false;

            foreach (var value in rows)
            {
                if (!activeRowSplit)
                {

                    if (value.Equals(QUOTE) || (value.StartsWith(QUOTE) && !value.EndsWith(QUOTE)))
                    {
                        data.Append(value);
                        activeRowSplit = true;
                        continue;
                    }


                    if ((!value.Contains(QUOTE)) || (value.StartsWith(QUOTE) && value.EndsWith(QUOTE)))
                    {
                        refinedRow.Add(value);
                        continue;
                    }
                }
                else if (activeRowSplit)
                {
                    if (!value.Contains(QUOTE))
                    {
                        data.Append($" {value}");
                        continue;
                    }

                    if (value.Equals(QUOTE) || value.EndsWith(QUOTE))
                    {
                        data.Append($" {value}");
                        refinedRow.Add(data.ToString());
                        data.Clear();
                        activeRowSplit = false;
                        continue;
                    }
                }
            }

            return refinedRow.ToArray();
        }
    }
}
