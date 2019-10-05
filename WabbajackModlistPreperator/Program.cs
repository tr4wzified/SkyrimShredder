using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace WabbajackLexyModlistPreperator
{
    class Program
    {
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

        static string Version = "1.0";

        // Application
        // Created on LexyLOTD v0.4.6
        static void Main(string[] args)
        {
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("Running trawzifieds Lexy LOTD Wabbajack Preperator");
            Console.WriteLine("Version " + Version);
            Console.WriteLine("-----------------------------------------------------------");
            string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Console.WriteLine("AppData Directory: " + AppData);
            string AppDataLocal = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            Console.WriteLine("AppData Local Directory: " + AppDataLocal);
            string ProgramFiles86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            Console.WriteLine("Program Files (x86) Directory: " + AppData);
            Console.WriteLine("-----------------------------------------------------------");

            // Step 1 
            Console.WriteLine("Do you wish to reinstall Skyrim Special Edition? (Y/N)");
            string ReInstall = Console.ReadLine().ToLower();
            if (ReInstall == "y" || ReInstall == "yes")
            {
                Console.WriteLine("Prompting user to uninstall Skyrim Special Edition.");
                Process.Start("steam://uninstall/489830");
                Console.WriteLine("Waiting for uninstallation...");
                Thread.Sleep(6500);
                Console.WriteLine("Prompting user to install Skyrim Special Edition.");
                Process.Start("steam://install/489830");
            }

            // Step 2
            string SSEModsFolder = Path.Combine(ProgramFiles86, "Steam", "steamapps", "common", "Skyrim Special Edition Mods");
            if (Directory.Exists(SSEModsFolder))
            {
                Console.WriteLine("Skyrim Special Edition Mods folder found! Do you wish to delete it? (Y/N)");
                string SSEMods = Console.ReadLine().ToLower();
                if (SSEMods == "y" || SSEMods == "yes")
                {
                    DeleteToRecyclingBin(SSEModsFolder);
                }
            Console.WriteLine("-----------------------------------------------------------");
            }
            else
            {
                Console.WriteLine("Skyrim Special Edition Mods folder not found.");
            }

            // Step 3
            string SSEConfigFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Skyrim Special Edition");
            if(Directory.Exists(SSEConfigFolder)) {
                Console.WriteLine("Skyrim Special Edition Documents folder found! WARNING: This folder might contain previous saves or configuration.");
                Console.WriteLine("You might want to back those up before removing this folder!");
                Console.WriteLine("Location: " + SSEConfigFolder);
                Console.WriteLine("Do you wish to delete it? (Y/N)");
                string SSEConfig = Console.ReadLine().ToLower();
                if(SSEConfig == "y" || SSEConfig == "yes")
                {
                    DeleteToRecyclingBin(SSEConfigFolder);
                }

            }
            else
            {
                Console.WriteLine("Skyrim Special Edition Documents folder not found.");
            }

            // Step 4
            if (Directory.Exists(Path.Combine(AppDataLocal, "LOOT"))) {
                Console.WriteLine("'AppData/Local/LOOT' folder found! Do you wish to delete it? (Y/N)");
                string LOOT = Console.ReadLine().ToLower();
                if(LOOT == "y" || LOOT == "yes")
                {
                    DeleteToRecyclingBin(Path.Combine(AppDataLocal, "LOOT"));
                }
            }
            else
            {
                Console.WriteLine("LOOT folder not found.");
            }

            // Step 5
            if (Directory.Exists(Path.Combine(AppDataLocal, "Skyrim Special Edition"))) {
                Console.WriteLine("'AppData/Local/Skyrim Special Edition' folder found! Do you wish to delete it? (Y/N)");
                string SSELocal = Console.ReadLine().ToLower();
                if(SSELocal == "y" || SSELocal == "yes")
                {
                    DeleteToRecyclingBin(Path.Combine(AppDataLocal, "Skyrim Special Edition"));
                }
            }
            else
            {
                Console.WriteLine("'AppData/Local/Skyrim Special Edition' folder not found.");
            }

            // Step 6

            if (Directory.Exists(Path.Combine(AppData, "zEdit"))) {
                Console.WriteLine("'AppData/Roaming/zEdit' folder found! Do you wish to delete it? (Y/N)");
                string zEdit = Console.ReadLine().ToLower();
                if(zEdit == "y" || zEdit == "yes")
                {
                    DeleteToRecyclingBin(Path.Combine(AppData, "zEdit"));
                }
            }
            else
            {
                Console.WriteLine("'AppData/Roaming/zEdit' folder not found.");
            }

            /*
            // Step 7
            if (Directory.Exists(Path.Combine(AppData, "Mod Organizer"))) {
                Console.WriteLine("'AppData/Roaming/Mod Organizer' folder found!");
                Console.WriteLine("WARNING: This folder might contain essential files from your previous modlist!");
                Console.WriteLine("I recommend backing it up somewhere else if you wish to keep it.");
                Console.WriteLine("Do you wish to delete it? (Y/N)");
                string MO = Console.ReadLine().ToLower();
                if(MO == "y" || MO == "yes")
                {
                    DeleteToRecyclingBin(Path.Combine(AppData, "Mod Organizer"));
                }
            }
            else
            {
                Console.WriteLine("'AppData/Roaming/Mod Organizer' folder not found.");
            }
            */

            // END
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("Preparation for modlist install complete! Press any key to exit.");
            Console.WriteLine("Now you're off to...");
            Console.WriteLine("The Wabbajack! Huh? Huh? Didn't see that coming, did you?");
            Console.WriteLine("-----------------------------------------------------------");
            Console.ReadKey();

        }
    }
}
