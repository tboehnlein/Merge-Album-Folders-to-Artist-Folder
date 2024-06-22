using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AlbumArtistMerge
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("This program requires a path to the folder with all of the album folders");

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();

                return;
            }

            string directoryPath = args[0];// @"M:\Soulseek Downloads\complete\__NEW\NOT DONE"; // Specify your directory path here

            if (Directory.Exists(directoryPath) == false)
            {
                Console.WriteLine($"Directory {directoryPath} does not exist");

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();

                return;
            }

            Console.WriteLine("Beginning moving album folders into artists folders");

            Console.WriteLine("Album folders must in the format of 'artist name - album name'");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            Console.WriteLine("");

            var folders = Directory.GetDirectories(directoryPath, "*", SearchOption.TopDirectoryOnly)
                                            .Select(Path.GetFileName)
                                            .ToList();

            var groupedFolders = folders
                .Select(folder => folder.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries))
                .GroupBy(splitFolder => splitFolder[0].Trim())
                .ToList();

            foreach (var group in groupedFolders)
            {
                var artistsFolder = group.Key;
                var albumGroup = group.ToList();

                if (albumGroup.First().Count() == 1)
                {
                    if (albumGroup.Count() == 1)
                    {
                        Console.WriteLine($"Skipping '{artistsFolder}' because it is an artist folder");
                        continue;
                    }
                    else
                    {
                        // Remove the artist folder from the list
                        albumGroup.RemoveAt(0);
                    }
                }

                foreach (var artistAlbumList in albumGroup)
                {
                    var albumFolder = string.Join(" - ", artistAlbumList);
                    var originalFolderPath = Path.Combine(directoryPath, albumFolder);
                    var artistPath = Path.Combine(directoryPath, artistsFolder);
                    var newFolderPath = Path.Combine(directoryPath, artistsFolder, albumFolder);                    
                    
                    if (!Directory.Exists(artistPath))
                    {
                        Console.WriteLine($"Creating missing artist folder: '{artistPath}'");
                        Directory.CreateDirectory(artistPath);
                    }

                    Console.WriteLine($"MOVING{Environment.NewLine}'{originalFolderPath}'{Environment.NewLine}INTO{Environment.NewLine}'{newFolderPath}'");
                    Directory.Move(originalFolderPath, newFolderPath);
                }
            }

            Console.WriteLine($"Done reorganizing album folders in {directoryPath}");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
