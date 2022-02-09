﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard_file_splitter_Core
{
    internal class FileHandling
    {
        internal static void WriteFile(string destinationFilePath, List<string> fileList)
        {
            File.WriteAllLines(destinationFilePath, fileList);
            Console.WriteLine($"{Path.GetFileName(destinationFilePath)} has been written in the {Path.GetDirectoryName(destinationFilePath)} Directory\n");
        }

        internal static void CopyFile(string sourceFilePath, string destinationFilePath)
        {
            File.Copy(sourceFilePath, destinationFilePath, true);
            Console.WriteLine($"{Path.GetFileName(sourceFilePath)} has been copied to the {Path.GetDirectoryName(destinationFilePath)} Directory\n");
        }

        internal static void MoveFile(string sourceFilePath, string destinationFilePath)
        {
            File.Move(sourceFilePath, destinationFilePath);
            Console.WriteLine($"{Path.GetFileName(sourceFilePath)} has been moved to the {Path.GetDirectoryName(destinationFilePath)} Directory\n");
        }

        internal static void ValidateDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
    }
}
