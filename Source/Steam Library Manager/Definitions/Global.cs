﻿using System.Collections.Generic;

namespace Steam_Library_Manager.Definitions
{
    internal static class Global
    {
        public static class Steam
        {
            public static readonly string RegistryKeyPath = @"HKEY_CURRENT_USER\SOFTWARE\Valve\Steam";

            public static string VdfFilePath = Alphaleonis.Win32.Filesystem.Path.Combine(Properties.Settings.Default.steamInstallationPath, "config", "config.vdf");

            public static bool IsStateChanging, Loaded;
        }

        public static class Origin
        {
            public static string ConfigFilePath = Alphaleonis.Win32.Filesystem.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "Origin", "local.xml");
            public static List<KeyValuePair<string, string>> AppIds = new List<KeyValuePair<string, string>>();

            public static bool IsStateChanging, Loaded;
        }

        public static class Uplay
        {
            public static readonly string LauncherRegistryPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Ubisoft\Launcher";
            public static readonly string InstallationsRegistryPath = @"SOFTWARE\Ubisoft\Launcher\Installs";

            public static string ConfigFilePath = Alphaleonis.Win32.Filesystem.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "Ubisoft Game Launcher", "settings.yml");

            public static bool IsStateChanging, Loaded;
        }
    }
}