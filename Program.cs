using System;

namespace PizzaFrenzySAFExtractor
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            string rootDirectoryForSaving = null;
            string safFilePath = null;
            
            if (args != null)
            {
                if (args.Length > 1)
                {
                    rootDirectoryForSaving = args[0];
                    safFilePath = args[1];
                }
                else if (args.Length == 1)
                {
                    safFilePath = args[0];
                }
            }

            if (string.IsNullOrEmpty(rootDirectoryForSaving))
            {
                Console.WriteLine("Enter the path to the root directory, where the extracted files will be placed.");
                Console.Write("Path: ");
                rootDirectoryForSaving = Console.ReadLine();
            }

            if (string.IsNullOrEmpty(safFilePath))
            {
                Console.WriteLine("Enter the path to the SAF file, which you want to extract.");
                Console.Write("Path to SAF: ");
                safFilePath = Console.ReadLine();
            }
            
            new SafExtractor(rootDirectoryForSaving, safFilePath, true).Extract();
            Console.WriteLine("Extracting completed.");
            Console.ReadKey();
        }
    }
}