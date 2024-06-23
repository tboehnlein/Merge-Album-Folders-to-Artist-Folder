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
            bool skipAnyKey = false;
            string directoryPath = string.Empty;

            if(!ProcessArguments(args, ref directoryPath, ref skipAnyKey))
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
                        Directory.CreateDirectory(artistPath);
                    }

                    Console.WriteLine($"MOVING{Environment.NewLine}'{originalFolderPath}'{Environment.NewLine}INTO{Environment.NewLine}'{newFolderPath}'");
                    Directory.Move(originalFolderPath, newFolderPath);
                }
            }

            Console.WriteLine($"Done reorganizing album folders in {directoryPath}");

            AskAnyKey(skipAnyKey);
        }

        private static bool ProcessArguments(string[] args, ref string directoryPath, ref bool skipAnyKey)
        {
            bool validArguments = true;

            switch (args.Length)
            {
                case 0:
                    Console.WriteLine("This program requires a path to the folder with all of the album folders");
                    AskAnyKey();
                    validArguments = false;
                    break;
                case 1:
                    directoryPath = args[0];
                    break;
                case 2:
                    if (args[0] == "-f")
                    {
                        skipAnyKey = true;
                        directoryPath = args[1];
                    }
                    else if (args[1] == "-f")
                    {
                        skipAnyKey = true;
                        directoryPath = args[0];
                    }
                    break;
                default:
                    Console.WriteLine("Too many arguments");
                    AskAnyKey();
                    validArguments = false;
                    break;
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
