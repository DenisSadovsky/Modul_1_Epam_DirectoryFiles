using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryFiles
{
    public class Program
    {
        static void Main(string[] args)
        {
            bool deletedFile = false;
            string path = "D:\\Books";
            Console.WriteLine("Input filter by date: ");
            var filter = Console.ReadLine().ToString();
            FileSystemVisitor fsv = filter == "" ? new FileSystemVisitor(new DirectoryInfo(path), new FileSystemProcessingAndFiltering())
                                                 : new FileSystemVisitor(new DirectoryInfo(path), new FileSystemProcessingAndFiltering() , filter);
            fsv.Start += (s, e) =>
            {
                Console.WriteLine("Iteration started");
            };

            fsv.Finish += (s, e) =>
            {
                Console.WriteLine("Iteration finished");
            };
            fsv.FileFinded += (s, e) =>
            {
                Console.WriteLine("\tFounded file: " + e.FindedItem.Name);
            };

            fsv.DirectoryFinded += (s, e) =>
            {
                Console.WriteLine("\tFounded directory: " + e.FindedItem.Name);
                if (e.FindedItem.Name.Length == 10)
                {
                    Console.WriteLine("Program stop");
                    e.ActionType = ActionType.StopSearch;
                }
            };

            fsv.FilteredFileFinded += (s, e) =>
            {
                Console.WriteLine("\t\tDeleted file after filtering : " + e.FindedItem.Name);
                deletedFile = true;
            };

            fsv.FilteredDirectoryFinded += (s, e) =>
            {
                Console.WriteLine("\t\tDeleted directory after filtering: " + e.FindedItem.Name);
                deletedFile = true;
                if (e.FindedItem.Name.Length == 10)
                { 
                    Console.WriteLine("Stop the program through the flag");
                    e.ActionType = ActionType.StopSearch;
                }
            };

            foreach (var fileSysInfo in fsv.GetFileSystemInfoSequence())
            {
                if (!deletedFile)
                {
                    Console.WriteLine(fileSysInfo);
                }
                deletedFile = false;
            }
            Console.Read();
        }   
    }
}
