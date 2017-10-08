﻿using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Steam_Library_Manager.Functions
{
    class Steam
    {
        public static void UpdatesteamInstallationPath()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.steamInstallationPath) || !System.IO.Directory.Exists(Properties.Settings.Default.steamInstallationPath))
            {
                Properties.Settings.Default.steamInstallationPath = Registry.GetValue(Definitions.Global.Steam.RegistryKeyPath, "SteamPath", "").ToString().Replace('/', Path.DirectorySeparatorChar);

                if (string.IsNullOrEmpty(Properties.Settings.Default.steamInstallationPath))
                {
                    if (MessageBox.Show("Steam couldn't be found under registry. Would you like to locate Steam manually?", "Steam installation couldn't be found", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        OpenFileDialog SteamPathSelector = new OpenFileDialog()
                        {
                            Filter = "Steam (Steam.exe)|Steam.exe"
                        };

                        if (SteamPathSelector.ShowDialog() == true)
                        {
                            Properties.Settings.Default.steamInstallationPath = Path.GetDirectoryName(SteamPathSelector.FileName);
                        }
                    }
                }

                Definitions.Global.Steam.vdfFilePath = Path.Combine(Properties.Settings.Default.steamInstallationPath, "config", "config.vdf");
            }
        }

        public static void PopulateLibraryCMenuItems()
        {
            #region App Context Menu Item Definitions

            // Open library in explorer ({0})
            Definitions.List.LibraryCMenuItems.Add(new Definitions.ContextMenuItem
            {
                Header = "Open library in explorer ({0})",
                Action = "Disk",
                Icon = FontAwesome.WPF.FontAwesomeIcon.FolderOpen,
                LibraryType = Definitions.Enums.LibraryType.Steam,
                ShowToOffline = false
            });

            // Separator
            Definitions.List.LibraryCMenuItems.Add(new Definitions.ContextMenuItem
            {
                IsSeparator = true,
                LibraryType = Definitions.Enums.LibraryType.Steam,
                ShowToOffline = false
            });

            // Remove library & files
            Definitions.List.LibraryCMenuItems.Add(new Definitions.ContextMenuItem
            {
                Header = "Remove library from Steam (/w files)",
                Action = "deleteLibrary",
                Icon = FontAwesome.WPF.FontAwesomeIcon.Trash,
                LibraryType = Definitions.Enums.LibraryType.Steam,
                ShowToOffline = false
            });

            // Delete games in library
            Definitions.List.LibraryCMenuItems.Add(new Definitions.ContextMenuItem
            {
                Header = "Delete games in library",
                Action = "deleteLibrarySLM",
                Icon = FontAwesome.WPF.FontAwesomeIcon.TrashOutline,
                LibraryType = Definitions.Enums.LibraryType.Steam,
                ShowToOffline = false
            });

            // Separator
            Definitions.List.LibraryCMenuItems.Add(new Definitions.ContextMenuItem
            {
                IsSeparator = true,
                ShowToNormal = false,
                LibraryType = Definitions.Enums.LibraryType.Steam,
                ShowToOffline = false
            });

            // Remove from SLM
            Definitions.List.LibraryCMenuItems.Add(new Definitions.ContextMenuItem
            {
                Header = "Remove from SLM",
                Action = "RemoveFromList",
                Icon = FontAwesome.WPF.FontAwesomeIcon.Minus,
                LibraryType = Definitions.Enums.LibraryType.Steam,
                ShowToNormal = false
            });

            #endregion
        }

        public static void PopulateAppCMenuItems()
        {
            #region App Context Menu Item Definitions

            // Run
            Definitions.List.AppCMenuItems.Add(new Definitions.ContextMenuItem
            {
                Header = "Run",
                Action = "steam://run/{0}",
                Icon = FontAwesome.WPF.FontAwesomeIcon.Play,
                LibraryType = Definitions.Enums.LibraryType.Steam,
                ShowToCompressed = false
            });

            // Separator
            Definitions.List.AppCMenuItems.Add(new Definitions.ContextMenuItem
            {
                ShowToCompressed = false,
                LibraryType = Definitions.Enums.LibraryType.Steam,
                IsSeparator = true
            });

            // Show on disk
            Definitions.List.AppCMenuItems.Add(new Definitions.ContextMenuItem
            {
                Header = "{0} ({1})",
                Action = "Disk",
                LibraryType = Definitions.Enums.LibraryType.Steam,
                Icon = FontAwesome.WPF.FontAwesomeIcon.FolderOpen
            });

            // View ACF
            Definitions.List.AppCMenuItems.Add(new Definitions.ContextMenuItem
            {
                Header = "View ACF File",
                Action = "acffile",
                Icon = FontAwesome.WPF.FontAwesomeIcon.PencilSquareOutline,
                LibraryType = Definitions.Enums.LibraryType.Steam,
                ShowToCompressed = false
            });

            // Game hub
            Definitions.List.AppCMenuItems.Add(new Definitions.ContextMenuItem
            {
                Header = "Game Hub",
                Action = "steam://url/GameHub/{0}",
                LibraryType = Definitions.Enums.LibraryType.Steam,
                Icon = FontAwesome.WPF.FontAwesomeIcon.Book
            });

            // Separator
            Definitions.List.AppCMenuItems.Add(new Definitions.ContextMenuItem
            {
                IsSeparator = true
            });

            // Workshop
            Definitions.List.AppCMenuItems.Add(new Definitions.ContextMenuItem
            {
                Header = "Workshop",
                Action = "steam://url/SteamWorkshopPage/{0}",
                LibraryType = Definitions.Enums.LibraryType.Steam,
                Icon = FontAwesome.WPF.FontAwesomeIcon.Cog
            });

            // Subscribed Workshop Items
            Definitions.List.AppCMenuItems.Add(new Definitions.ContextMenuItem
            {
                Header = "Subscribed Workshop Items",
                Action = "https://steamcommunity.com/profiles/{1}/myworkshopfiles/?appid={0}&browsefilter=mysubscriptions&sortmethod=lastupdated",
                LibraryType = Definitions.Enums.LibraryType.Steam,
                Icon = FontAwesome.WPF.FontAwesomeIcon.Cogs
            });

            // Separator
            Definitions.List.AppCMenuItems.Add(new Definitions.ContextMenuItem
            {
                IsSeparator = true,
                LibraryType = Definitions.Enums.LibraryType.Steam
            });

            // Subscribed Workshop Items
            Definitions.List.AppCMenuItems.Add(new Definitions.ContextMenuItem
            {
                Header = "Delete files (using SLM)",
                Action = "deleteappfiles",
                LibraryType = Definitions.Enums.LibraryType.Steam,
                Icon = FontAwesome.WPF.FontAwesomeIcon.Trash
            });

            #endregion
        }

        public static bool IsSteamWorking()
        {
            try
            {
                Process[] SteamProcesses = Process.GetProcessesByName("Steam");

                if (SteamProcesses.Length > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogToFile(Logger.LogType.SLM, ex.ToString());
                return true;
            }

        }

        public static async Task CloseSteamAsync()
        {
            try
            {
                if (IsSteamWorking())
                {
                    if (MessageBox.Show("Steam needs to be closed for this action. Would you like SLM to close Steam?", "Steam needs to be closed", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        if (File.Exists(Path.Combine(Properties.Settings.Default.steamInstallationPath, "steam.exe")))
                        {
                            Process.Start($"{Path.Combine(Properties.Settings.Default.steamInstallationPath, "steam.exe")}", "-shutdown");
                        }
                        else if (MessageBox.Show("Steam.exe could not found, SLM will try to terminate Steam processes now.", "Steam needs to be closed", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            Process[] SteamProcesses = Process.GetProcessesByName("Steam");

                            foreach (Process SteamProcess in SteamProcesses)
                            {
                                SteamProcess.Kill();
                            }
                        }
                        else
                        {
                            throw new Exception("Steam.exe could not found and user doesn't wants to terminate the process.");
                        }
                    }
                    else
                    {
                        throw new Exception("User doesn't wants to close Steam, can not continue to process.");
                    }

                    await Task.Delay(6000);
                }
            }
            catch (Exception ex)
            {
                Logger.LogToFile(Logger.LogType.SLM, ex.ToString());
                MessageBox.Show(ex.Message, ex.Source);
            }
        }

        public static async void RestartSteamAsync()
        {
            try
            {
                if (MessageBox.Show("Would you like to Restart Steam?", "Restart Steam?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    await CloseSteamAsync();

                    if (File.Exists(Path.Combine(Properties.Settings.Default.steamInstallationPath, "steam.exe")))
                    {
                        Process.Start($"{Path.Combine(Properties.Settings.Default.steamInstallationPath, "steam.exe")}");
                    }
                }
                else
                {
                    throw new Exception("User doesn't wants to restart Steam.");
                }
            }
            catch (Exception ex)
            {
                Logger.LogToFile(Logger.LogType.SLM, ex.ToString());
            }
        }

        public class Library
        {
            public static async void CreateNew(string NewLibraryPath, bool Backup)
            {
                try
                {
                    // If we are not creating a backup library
                    if (!Backup)
                    {
                        await Steam.CloseSteamAsync();

                        // Define steam dll paths for better looking
                        string SteamDLLPath = Path.Combine(NewLibraryPath, "Steam.dll");

                        if (!File.Exists(SteamDLLPath))
                        {
                            // Copy Steam.dll as steam needs it
                            File.Copy(Path.Combine(Properties.Settings.Default.steamInstallationPath, "Steam.dll"), SteamDLLPath, true);
                        }

                        if (!Directory.Exists(Path.Combine(NewLibraryPath, "SteamApps")))
                        {
                            // create SteamApps directory at requested directory
                            Directory.CreateDirectory(Path.Combine(NewLibraryPath, "SteamApps"));
                        }

                        // If Steam.dll moved succesfully
                        if (File.Exists(SteamDLLPath)) // in case of permissions denied
                        {
                            // Call KeyValue in act
                            Framework.KeyValue Key = new Framework.KeyValue();

                            // Read vdf file as text
                            Key.ReadFileAsText(Definitions.Global.Steam.vdfFilePath);

                            // Add our new library to vdf file so steam will know we have a new library
                            Key["Software"]["Valve"]["Steam"].Children.Add(new Framework.KeyValue(string.Format("BaseInstallFolder_{0}", Definitions.List.Libraries.Select(x => x.Type == Definitions.Enums.LibraryType.Steam).Count()), NewLibraryPath));

                            // Save vdf file
                            Key.SaveToFile(Definitions.Global.Steam.vdfFilePath, false);

                            // Show a messagebox to user about process
                            MessageBox.Show("New Steam library created");

                            // Since this file started to interrupt us? 
                            // No need to bother with it since config.vdf is the real deal, just remove it and Steam client will handle.
                            if (File.Exists(Path.Combine(Properties.Settings.Default.steamInstallationPath, "steamapps", "libraryfolders.vdf")))
                            {
                                File.Delete(Path.Combine(Properties.Settings.Default.steamInstallationPath, "steamapps", "libraryfolders.vdf"));
                            }

                            Steam.RestartSteamAsync();
                        }
                        else
                        {
                            // Show an error to user and cancel the process because we couldn't get Steam.dll in new library dir
                            MessageBox.Show("Failed to copy Steam.dll into new library directory. Permission error, maybe?");
                        }
                    }

                    // Add library to list
                    AddNew(NewLibraryPath, false);

                    // Save our settings
                    SLM.Settings.SaveSettings();
                }
                catch (Exception ex)
                {
                    Logger.LogToFile(Logger.LogType.Library, ex.ToString());
                    MessageBox.Show(ex.ToString());
                }
            }

            public static void CheckForBackupUpdates()
            {
                try
                {
                    if (Definitions.List.Libraries.Count(x => x.Type == Definitions.Enums.LibraryType.SLM && x.DirectoryInfo.Exists) == 0)
                    {
                        return;
                    }

                    foreach (Definitions.Library CurrentLibrary in Definitions.List.Libraries.Where(x => x.Type == Definitions.Enums.LibraryType.SLM && x.DirectoryInfo.Exists))
                    {
                        if (CurrentLibrary.Steam.Apps.Count == 0)
                        {
                            continue;
                        }

                        foreach (Definitions.Library LibraryToCheck in Definitions.List.Libraries.Where(x => x.Type == Definitions.Enums.LibraryType.Steam))
                        {
                            foreach (Definitions.AppInfo LatestApp in CurrentLibrary.Steam.Apps)
                            {
                                if (LibraryToCheck.Steam.Apps.Count(x => x.AppID == LatestApp.AppID && x.LastUpdated < LatestApp.LastUpdated) > 0)
                                {
                                    Definitions.AppInfo OldAppBackup = LibraryToCheck.Steam.Apps.First(x => x.AppID == LatestApp.AppID && x.LastUpdated < LatestApp.LastUpdated);

                                    if (Framework.TaskManager.TaskList.Count(x => x.TargetApp.AppID == LatestApp.AppID && x.Library == OldAppBackup.Library && !x.Completed) == 0)
                                    {
                                        Definitions.List.TaskList NewTask = new Definitions.List.TaskList
                                        {
                                            TargetApp = LatestApp,
                                            Library = OldAppBackup.Library
                                        };

                                        Framework.TaskManager.AddTask(NewTask);
                                    }

                                    Main.FormAccessor.TaskManager_Logs.Add($"[{DateTime.Now}] An update is available for: {LatestApp.AppName} - Old backup time: {OldAppBackup.LastUpdated} - Updated on: {LatestApp.LastUpdated} - Target: {LatestApp.Library.Steam.FullPath} - Source: {OldAppBackup.Library.Steam.FullPath}");
                                }
                            }
                        }
                    }

                    Main.FormAccessor.TaskManager_Logs.Add($"[{DateTime.Now}] Checked for Backup updates.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    Logger.LogToFile(Logger.LogType.Library, ex.ToString());
                }
            }

            public static async void AddNew(string LibraryPath, bool IsMainLibrary)
            {
                try
                {
                    Definitions.SteamLibrary Library = new Definitions.SteamLibrary()
                    {
                        IsMain = IsMainLibrary,
                        FullPath = LibraryPath
                    };

                    Definitions.List.Libraries.Add(new Definitions.Library
                    {
                        Type = Definitions.Enums.LibraryType.Steam,
                        DirectoryInfo = new DirectoryInfo(LibraryPath),
                        Steam = Library
                    });

                    await Task.Run(() => Library.UpdateAppList());
                    await Task.Run(() => Library.UpdateJunks());
                }
                catch (Exception ex)
                {
                    Logger.LogToFile(Logger.LogType.Library, ex.ToString());
                    MessageBox.Show(ex.ToString());
                }
            }

            public static void GenerateLibraryList()
            {
                try
                {
                    if (File.Exists(Path.Combine(Properties.Settings.Default.steamInstallationPath, "Steam.exe")))
                    {
                        AddNew(Properties.Settings.Default.steamInstallationPath, true);
                    }

                    // Make a KeyValue reader
                    Framework.KeyValue KeyValReader = new Framework.KeyValue();

                    // If config.vdf exists
                    if (File.Exists(Definitions.Global.Steam.vdfFilePath))
                    {
                        // Read our vdf file as text
                        KeyValReader.ReadFileAsText(Definitions.Global.Steam.vdfFilePath);

                        KeyValReader = KeyValReader["Software"]["Valve"]["Steam"];
                        if (KeyValReader.Children.Count > 0)
                        {
                            Definitions.SLM.UserSteamID64 = (KeyValReader["Accounts"].Children.Count > 0) ? KeyValReader["Accounts"].Children[0].Children[0].Value : null;

                            foreach (Framework.KeyValue key in KeyValReader.Children.FindAll(x => x.Name.Contains("BaseInstallFolder")))
                            {
                                AddNew(key.Value, false);
                            }
                        }
                    }
                    else { /* Could not locate LibraryFolders.vdf */ }
                }
                catch (Exception ex)
                {
                    Logger.LogToFile(Logger.LogType.Library, ex.ToString());
                    MessageBox.Show(ex.ToString());
                }
            }

            public static bool IsLibraryExists(string NewLibraryPath)
            {
                try
                {
                    NewLibraryPath = NewLibraryPath.ToLowerInvariant();

                    if (Definitions.List.Libraries.Count(x => x.Type == Definitions.Enums.LibraryType.Steam) > 0)
                    {
                        if (Definitions.List.Libraries.Where(x => x.Steam.FullPath.ToLowerInvariant() == NewLibraryPath ||
                        x.Steam.CommonFolder.FullName.ToLowerInvariant() == NewLibraryPath ||
                        x.Steam.DownloadFolder.FullName.ToLowerInvariant() == NewLibraryPath ||
                        x.Steam.WorkshopFolder.FullName.ToLowerInvariant() == NewLibraryPath ||
                        x.Steam.SteamAppsFolder.FullName.ToLowerInvariant() == NewLibraryPath).Count() > 0)
                        {
                            return true;
                        }
                    }

                    // else, return false which means library is not exists
                    return false;
                }
                // In any error return true to prevent possible bugs
                catch (Exception ex)
                {
                    Logger.LogToFile(Logger.LogType.Library, ex.ToString());
                    MessageBox.Show(ex.ToString());
                    return true;
                }
            }

        }
    }
}
