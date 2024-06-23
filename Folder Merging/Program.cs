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
        /// <summary>
        /// Command line program that moves album folders formated as 'artist - album' into artist folders
        /// </summary>
        /// <param name="args">path to folder that contains all of the album folders, force skip press any key for non-errors</param>
        static void Main(string[] args)
        {
            if(!ProcessArguments(args, out string directoryPath, out bool skipAnyKey, out bool simulate))
            {
                return;
            }

            if (Directory.Exists(directoryPath) == false)
            {
                Console.WriteLine($"Directory {directoryPath} does not exist");

                AskAnyKey();

                return;
            }

            Console.WriteLine("Beginning moving album folders into artists folders");
            Console.WriteLine("Album folders must in the format of 'artist name - album name'");

            if(simulate)
            {
                Console.WriteLine("Simulating moving folders");
            }

            AskAnyKey(skipAnyKey);

            Console.WriteLine("");

            var folders = Directory.GetDirectories(directoryPath, "*", SearchOption.TopDirectoryOnly)
                                            .Select(Path.GetFileName)
                                            .ToList();

            // create a list of folders grouped by artist name
            // each artist name will have a list of split folder names
            // if the split folder name count is 1, then it is an artist folder
            var groupedFolders = folders
                .Select(folder => folder.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries))
                .GroupBy(splitFolder => splitFolder[0].Trim())
                .ToList();

            foreach (var group in groupedFolders)
            {
                var artistsFolder = group.Key;
                var albumGroup = group.ToList();

                // if the split folder list count is 1, then it is an artist folder
                if (albumGroup.First().Count() == 1)
                {
                    // if no album folders are found, skip the artist folder
                    // otherwise, remove the artist folder from the list
                    if (albumGroup.Count() == 1)
                    {
                        Console.WriteLine($"Skipping '{artistsFolder}' because it is an artist folder");
                        continue;
                    }
                    else
                    {
                        albumGroup.RemoveAt(0);
                    }
                }

                // for each artist, move the album folders into the artist folder
                foreach (var artistAlbumList in albumGroup)
                {
                    var albumFolder = string.Join(" - ", artistAlbumList);
                    var originalFolderPath = Path.Combine(directoryPath, albumFolder);
                    var artistPath = Path.Combine(directoryPath, artistsFolder);
                    var newFolderPath = Path.Combine(directoryPath, artistsFolder, albumFolder);                    
                    
                    if (!Directory.Exists(artistPath))
                    {
                        Console.WriteLine($"Creating missing artist folder: '{artistPath}'");
                        
                        if (!simulate)
                        {
                            Directory.CreateDirectory(artistPath);
                        }
                    }

                    Console.WriteLine($"MOVING{Environment.NewLine}'{originalFolderPath}'{Environment.NewLine}INTO{Environment.NewLine}'{newFolderPath}'");

                    if (!simulate)
                    {
                        Directory.Move(originalFolderPath, newFolderPath);
                    }
                }
            }

            Console.WriteLine($"Done reorganizing album folders in {directoryPath}");

            AskAnyKey(skipAnyKey);
        }

        private static bool ProcessArguments(string[] args, out string directoryPath, out bool skipAnyKey, out bool simulate)
        {
            bool validArguments = true;
            directoryPath = string.Empty;
            skipAnyKey = false;
            simulate = false;

            if (args.Contains("-h") || args.Contains("--help"))
            {
                Console.WriteLine("This program moves album folders formated as 'artist - album' into a single artist folders");
                Console.WriteLine("Usage: AlbumArtistMerge.exe [-f] [-s] [-h] <path to folder with all of the album folders>");
                Console.WriteLine("Options:");
                Console.WriteLine("  -f  force skip press any key for non-errors");
                Console.WriteLine("  -s  simulate organizing files");
                Console.WriteLine("  -h  display this help message");
                AskAnyKey();
                return false;
            }

            if (args.Contains("-f") || args.Contains("--force"))
            { 
                skipAnyKey = true;
            }

            if (args.Contains("-s") || args.Contains("--simulate"))
            {
                simulate = true;
            }

            foreach (var arg in args)
            {
                if (arg.Contains(Path.DirectorySeparatorChar))
                {
                    directoryPath = arg;
                    break;
                }
            }

            if (string.IsNullOrEmpty(directoryPath))
            {
                Console.WriteLine("No directory path provided");
                validArguments = false;
            }

            return validArguments;
        }

        private static void AskAnyKey(bool skipAnyKey = false)
        {
            if (!skipAnyKey)
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }
    }
}
