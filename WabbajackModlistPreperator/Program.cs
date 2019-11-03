using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace SkyrimShredder
{
    class Program
    {
        private static string Version = "1.3";

        //
        // Moving files to recycle bin instead of deleting to prevent catastrophy.
        //

        private const int FO_DELETE = 0x0003;
        private const int FOF_ALLOWUNDO = 0x0040;           // Preserve undo information, if possible. 
        private const int FOF_NOCONFIRMATION = 0x0010;      // Show no confirmation dialog box to the user

        // Struct which contains information that the SHFileOperation function uses to perform file operations. 
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHFILEOPSTRUCT
        {
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.U4)]
            public int wFunc;
            public string pFrom;
            public string pTo;
            public short fFlags;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            public string lpszProgressTitle;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);
        public static void DeleteToRecyclingBin(string path)
        {
            SHFILEOPSTRUCT fileop = new SHFILEOPSTRUCT();
            fileop.wFunc = FO_DELETE;
            fileop.pFrom = path + '\0' + '\0';
            fileop.fFlags = FOF_ALLOWUNDO | FOF_NOCONFIRMATION;
            SHFileOperation(ref fileop);
        }


        // Main application
        static void Main(string[] args)
        {

            string SSEFolder = "";
            string SSEModsFolder = "";
            string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string AppDataLocal = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string SteamFolder = (string)Registry.CurrentUser.OpenSubKey(@"Software\\Valve\\Steam", false).GetValue("SteamPath");

            List<string> SteamLibraries = new List<string>();
            // This library always exists when installing Steam, and is not in libraryfolders.vdf, so adding it manually
            SteamLibraries.Add(Path.Combine(SteamFolder, "steamapps", "common"));

            foreach (string line in File.ReadLines(Path.Combine(SteamFolder, "steamapps", "libraryfolders.vdf")))
            {
                string a = line.Trim();
                if (a.Length > 2)
                {
                    if (Char.IsDigit(a[1]))
                    {
                        string[] b = a.Split('"');
                        SteamLibraries.Add(Path.Combine(b[3], "steamapps", "common"));
                    }
                }
            }

            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("Running trawzifieds Skyrim Shredder");
            Console.WriteLine("Version " + Version);
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("AppData Directory: " + AppData);
            Console.WriteLine("AppData Local Directory: " + AppDataLocal);

            // Search for all Steam Libraries on this computer
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("Detected Steam Libraries: ");
            foreach (string SteamLibrary in SteamLibraries)
            {
                Console.WriteLine(SteamLibrary);
                if (Directory.Exists(Path.Combine(SteamLibrary, "Skyrim Special Edition")))
                {
                    SSEFolder = Path.Combine(SteamLibrary, "Skyrim Special Edition");
                }
                if (Directory.Exists(Path.Combine(SteamLibrary, "Skyrim Special Edition Mods")))
                {
                    SSEModsFolder = Path.Combine(SteamLibrary, "Skyrim Special Edition Mods");
                }
            }
            Console.WriteLine("-----------------------------------------------------------");

            // Step 1 
            if (SSEFolder != null)
            {
                Console.WriteLine("Do you wish to reinstall Skyrim Special Edition? (Y/N)");
                string ReInstall = Console.ReadLine().ToLower();
                if (ReInstall == "y" || ReInstall == "yes")
                {
                    DeleteToRecyclingBin(SSEFolder);
                    Console.WriteLine("Prompting user to uninstall Skyrim Special Edition.");
                    Process.Start("steam://uninstall/489830");
                    Console.WriteLine("Waiting for uninstallation...");
                    Thread.Sleep(2500);
                    Console.WriteLine("Prompting user to install Skyrim Special Edition.");
                    Process.Start("steam://install/489830");
                }
            }
            else
            {
                Console.WriteLine("Skyrim Special Edition not found.");
            }

            // Step 2

            if (SSEModsFolder != null)
            {
                Console.WriteLine("Skyrim Special Edition Mods folder found! Do you wish to delete it? (Y/N)");
                string SSEMods = Console.ReadLine().ToLower();
                if (SSEMods == "y" || SSEMods == "yes")
                {
                    DeleteToRecyclingBin(SSEModsFolder);
                }
            }
            else
            {
                Console.WriteLine("Skyrim Special Edition Mods folder not found.");
            }

            // Step 3
            string SSEConfigFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Skyrim Special Edition");
            if (Directory.Exists(SSEConfigFolder))
            {
                Console.WriteLine("Skyrim Special Edition Documents folder found!");
                Console.WriteLine("WARNING: This folder might contain previous saves or configuration.");
                Console.WriteLine("You might want to back those up before removing this folder!");
                Console.WriteLine("Location: " + SSEConfigFolder);
                Console.WriteLine("Do you wish to delete it? (Y/N)");
                string SSEConfig = Console.ReadLine().ToLower();
                if (SSEConfig == "y" || SSEConfig == "yes")
                {
                    DeleteToRecyclingBin(SSEConfigFolder);
                }

            }
            else
            {
                Console.WriteLine("Skyrim Special Edition Documents folder not found.");
            }

            // Step 4
            if (Directory.Exists(Path.Combine(AppDataLocal, "LOOT")))
            {
                Console.WriteLine("'AppData/Local/LOOT' folder found! Do you wish to delete it? (Y/N)");
                string LOOT = Console.ReadLine().ToLower();
                if (LOOT == "y" || LOOT == "yes")
                {
                    DeleteToRecyclingBin(Path.Combine(AppDataLocal, "LOOT"));
                }
            }
            else
            {
                Console.WriteLine("LOOT folder not found.");
            }

            // Step 5
            if (Directory.Exists(Path.Combine(AppDataLocal, "Skyrim Special Edition")))
            {
                Console.WriteLine("'AppData/Local/Skyrim Special Edition' folder found! Do you wish to delete it? (Y/N)");
                string SSELocal = Console.ReadLine().ToLower();
                if (SSELocal == "y" || SSELocal == "yes")
                {
                    DeleteToRecyclingBin(Path.Combine(AppDataLocal, "Skyrim Special Edition"));
                }
            }
            else
            {
                Console.WriteLine("'AppData/Local/Skyrim Special Edition' folder not found.");
            }

            // Step 6
            Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.MyComputer).ToString());
            if(Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyComputer))) { }

            if (Directory.Exists(Path.Combine(AppData, "zEdit")))
            {
                Console.WriteLine("'AppData/Roaming/zEdit' folder found! Do you wish to delete it? (Y/N)");
                string zEdit = Console.ReadLine().ToLower();
                if (zEdit == "y" || zEdit == "yes")
                {
                    DeleteToRecyclingBin(Path.Combine(AppData, "zEdit"));
                }
            }
            else
            {
                Console.WriteLine("'AppData/Roaming/zEdit' folder not found.");
            }

            // END
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("Skyrim Shredder complete! Press any key to exit.");
            Console.WriteLine("-----------------------------------------------------------");
            Console.ReadKey();

        }
    }
}
