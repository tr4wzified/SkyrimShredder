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
        private static string Version = "1.6";

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
            string ProgramFiles = Environment.ExpandEnvironmentVariables("%ProgramW6432%");
            string ProgramFilesX86 = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%");
            string Documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

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

            // Search for all Steam Libraries on this computer
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

            // Skyrim SE Game Directory
            if (SSEFolder != null)
            {
                Console.WriteLine("Do you wish to reinstall Skyrim Special Edition? (Y/N)");
                string ReInstall = Console.ReadLine().ToLower();
                if (ReInstall == "y" || ReInstall == "yes")
                {
                    Console.WriteLine("Prompting Steam uninstallation process for Skyrim Special Edition.");
                    Process SteamUninstallation = Process.Start("steam://uninstall/489830");
                    Console.WriteLine("Waiting for uninstallation...");
                    // Steam doesn't work with Process.WaitForExit(), kinda weird hack but guess we'll wait for 5 seconds then.
                    Thread.Sleep(5000);

                    // Remove all Skyrim SE installations, 
                    foreach (string SteamLibrary in SteamLibraries)
                    {
                        if (Directory.Exists(Path.Combine(SteamLibrary, "Skyrim Special Edition")))
                        {
                            DeleteToRecyclingBin(Path.Combine(SteamLibrary, "Skyrim Special Edition"));
                        }
                    }
                    Console.WriteLine("Prompting Steam installation process for Skyrim Special Edition.");
                    Process.Start("steam://install/489830");
                }
            }
            else
            {
                Console.WriteLine("Skyrim Special Edition not found.");
            }

            // Skyrim SE Mods next to game directory

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

            // Skyrim SE My Documents
            string SSEConfigFolder = Path.Combine(Documents, "My Games", "Skyrim Special Edition");
            if (Directory.Exists(SSEConfigFolder))
            {
                Console.WriteLine("Skyrim Special Edition Documents folder found!");
                Console.WriteLine("WARNING: This folder might contain previous saves or configuration.");
                Console.WriteLine("You might want to back those up before removing this folder!");
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

            // LOOT
            if (Directory.Exists(Path.Combine(AppDataLocal, "LOOT")) || Directory.Exists(Path.Combine(ProgramFilesX86, "LOOT")))
            {
                Console.WriteLine("LOOT found! Do you wish to delete LOOT? (Y/N)");
                string LOOT = Console.ReadLine().ToLower();
                if (LOOT == "y" || LOOT == "yes")
                {
                    if(Directory.Exists(Path.Combine(AppDataLocal, "LOOT")))
                    {
                        DeleteToRecyclingBin(Path.Combine(AppDataLocal, "LOOT"));
                    }

                    if (Directory.Exists(Path.Combine(ProgramFilesX86, "LOOT"))) {
                        try
                        {
                            Process LOOTUninstaller = Process.Start(Path.Combine(ProgramFilesX86, "LOOT", "unins001.exe"));
                            LOOTUninstaller.WaitForExit();
                        }
                        catch
                        {
                            Console.WriteLine("LOOT uninstaller not found! Exiting in 5 seconds.");
                            Thread.Sleep(5000);
                            Environment.Exit(0);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("LOOT folder not found.");
            }

            // Nexus Mod Manager
            if (Directory.Exists(Path.Combine(ProgramFiles, "Nexus Mod Manager")))
            {
                Console.WriteLine("Nexus Mod Manager found! Do you wish to delete NMM? (Y/N)");
                string NexusModManager = Console.ReadLine().ToLower();
                if (NexusModManager == "y" || NexusModManager == "yes")
                {
                    var LocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                    string NMMInstallInfo = (string)LocalMachine.OpenSubKey(@"SOFTWARE\\NexusModManager\\SkyrimSE", false).GetValue("InstallInfo");
                    string NMMMods = (string)LocalMachine.OpenSubKey(@"SOFTWARE\\NexusModManager\\SkyrimSE", false).GetValue("Mods");
                    string NMMVirtual = (string)LocalMachine.OpenSubKey(@"SOFTWARE\\NexusModManager\\SkyrimSE", false).GetValue("Virtual");
                    try
                    {
                        Process NMMUninstaller = Process.Start(Path.Combine(ProgramFiles, "Nexus Mod Manager", "uninstall", "unins000.exe"));
                        NMMUninstaller.WaitForExit();
                    }
                    catch
                    {
                        Console.WriteLine("Could not find Nexus Mod Manager uninstaller! Exiting in 5 seconds.");
                        Thread.Sleep(5000);
                        Environment.Exit(0);
                    }

                    // This one is not removed by the uninstaller for some reason
                    if (Directory.Exists(Path.Combine(ProgramFiles, "Nexus Mod Manager")))
                    {
                        DeleteToRecyclingBin(Path.Combine(ProgramFiles, "Nexus Mod Manager"));
                    }
                    // These should be gone already but checking just in case
                    if (Directory.Exists(NMMInstallInfo))
                    {
                        DeleteToRecyclingBin(NMMInstallInfo);
                    }

                    if (Directory.Exists(NMMMods))
                    {
                        DeleteToRecyclingBin(NMMMods);
                    }

                    if (Directory.Exists(NMMVirtual))
                    {
                        DeleteToRecyclingBin(NMMVirtual);
                    }
                }
            }
            else
            {
                Console.WriteLine("Nexus Mod Manager not found.");
            }

            // Skyrim SE AppDataLocal
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

            // zEdit AppData
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
