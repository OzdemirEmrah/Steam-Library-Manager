﻿using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Steam_Library_Manager.Forms
{
    public partial class MoveGame : Form
    {
        public MoveGame()
        {
            InitializeComponent();
        }

        Definitions.List.GamesList Game = Definitions.SLM.LatestSelectedGame;
        Definitions.List.LibraryList Library = Definitions.SLM.LatestDropLibrary;

        Stopwatch TimeElapsed = new Stopwatch();

        private void MoveGame_Load(object sender, System.EventArgs e)
        {
            pictureBox_GameImage.LoadAsync("http://cdn.akamai.steamstatic.com/steam/apps/"+ Game.appID +"/header.jpg");
            linkLabel_currentLibrary.Text = Game.Library.Directory;
            linkLabel_TargetLibrary.Text = Library.Directory;

            long NeededSpace = 0;

            if (Game.exactInstallPath != null)
                NeededSpace += Functions.FileSystem.GetDirectorySize(Game.exactInstallPath, true);

            if (Game.downloadPath != null)
                NeededSpace += Functions.FileSystem.GetDirectorySize(Game.downloadPath, true);

            label_AvailableSpace.Text = Functions.FileSystem.FormatBytes(Functions.FileSystem.GetFreeSpace(Library.Directory));
            label_NeededSpace.Text = Functions.FileSystem.FormatBytes(NeededSpace);
        }

        private void button_Copy_Click(object sender, EventArgs e)
        {
            try
            {
                button_Copy.Enabled = false;
                CopyGame(this.checkbox_Validate.Checked, this.checkbox_RemoveOldFiles.Checked);
            }
            catch { }
        }

        async void CopyGame(bool Validate, bool RemoveOld)
        {
            string downloadPath = Game.Library.Directory + @"downloading\";
            string TargetGamePath = Library.Directory + @"common\" + Game.installationPath;
            string TargetDownloadPath = Library.Directory + @"downloading\" + Game.appID;
            string acfName = "appmanifest_" + Game.appID + ".acf";
            try
            {
                TimeElapsed.Start();
                timer_TimeElapsed.Start();

                // If we have create & remove permissions at the target game path
                if (Functions.FileSystem.TestFile(TargetGamePath))
                {
                    // If something is wrong and current game directory doesn't exists
                    if (!Directory.Exists(Game.exactInstallPath) && !Directory.Exists(Game.downloadPath))
                    {
                        // Show error to user
                        System.Windows.Forms.MessageBox.Show("Can not find selected game files... Is there something went wrong with coding?\nDirectory: " + Game.exactInstallPath);
                        return;
                    }

                    long freeSpace = Functions.FileSystem.GetFreeSpace(TargetGamePath);

                    if (freeSpace < Game.sizeOnDisk)
                    {
                        Log("Free space is not enough! Needed Free Space: " + Game.sizeOnDisk + " Available: " + freeSpace);
                        return;
                    }
                    // Create the directory
                    Directory.CreateDirectory(TargetGamePath);

                    byte[] currentFileMD5, newFileMD5;
                    string newFileName;
                    int FilesToMove = 0, movedFiles = 0;

                    if (Game.exactInstallPath != null)
                        FilesToMove += Framework.FastDirectoryEnumerator.GetFiles(Game.exactInstallPath, "*", SearchOption.AllDirectories).Length;

                    if (Game.downloadPath != null)
                        FilesToMove += Framework.FastDirectoryEnumerator.GetFiles(Game.downloadPath, "*", SearchOption.AllDirectories).Length;

                    progressBar_CopyStatus.Maximum = FilesToMove;

                    // common
                    if (Game.exactInstallPath != null)
                    {
                        foreach (Framework.FileData currentFile in Framework.FastDirectoryEnumerator.EnumerateFiles(Game.exactInstallPath, "*", SearchOption.AllDirectories))
                        {
                            using (FileStream currentFileStream = File.Open(currentFile.Path, FileMode.Open, FileAccess.Read))
                            {
                                newFileName = TargetGamePath + currentFile.Path.Replace(Game.exactInstallPath, "");
                                Directory.CreateDirectory(Path.GetDirectoryName(newFileName));
                                using (FileStream newFileStream = File.Create(newFileName))
                                {
                                    await currentFileStream.CopyToAsync(newFileStream);

                                    movedFiles += 1;
                                }
                            }

                            if (Validate)
                            {
                                currentFileMD5 = Functions.FileSystem.GetFileMD5(currentFile.Path);
                                newFileMD5 = Functions.FileSystem.GetFileMD5(newFileName);
                                if (BitConverter.ToString(currentFileMD5) != BitConverter.ToString(newFileMD5))
                                {
                                    Log("[" + movedFiles.ToString() + "/" + FilesToMove.ToString() + "] File couldn't verified: " + currentFile.Path.Replace(Game.exactInstallPath, ""));
                                    break;
                                }

                            }

                            Log("[" + movedFiles.ToString() + "/" + FilesToMove.ToString() + "] Copied: " + currentFile.Path.Replace(Game.exactInstallPath, ""));
                            progressBar_CopyStatus.PerformStep();
                        }
                    }

                    // downloading
                    if (Game.downloadPath != null)
                    {
                        foreach (Framework.FileData currentFile in Framework.FastDirectoryEnumerator.EnumerateFiles(Game.downloadPath, "*", SearchOption.AllDirectories))
                        {
                            using (FileStream currentFileStream = File.Open(currentFile.Path, FileMode.Open, FileAccess.Read))
                            {
                                newFileName = TargetDownloadPath + currentFile.Path.Replace(Game.downloadPath, "");
                                Directory.CreateDirectory(Path.GetDirectoryName(newFileName));
                                using (FileStream newFileStream = File.Create(newFileName))
                                {
                                    await currentFileStream.CopyToAsync(newFileStream);

                                    movedFiles += 1;
                                }
                            }

                            if (Validate)
                            {
                                currentFileMD5 = Functions.FileSystem.GetFileMD5(currentFile.Path);
                                newFileMD5 = Functions.FileSystem.GetFileMD5(newFileName);
                                if (BitConverter.ToString(currentFileMD5) != BitConverter.ToString(newFileMD5))
                                {
                                    Log("[" + movedFiles.ToString() + "/" + FilesToMove.ToString() + "] File couldn't verified: " + currentFile.Path.Replace(Game.downloadPath, ""));
                                    break;
                                }

                            }

                            Log("[" + movedFiles.ToString() + "/" + FilesToMove.ToString() + "] Copied: " + currentFile.Path.Replace(Game.downloadPath, ""));
                            progressBar_CopyStatus.PerformStep();
                        }
                    }

                    // .Patch files
                    Directory.CreateDirectory(Library.Directory + @"downloading\");
                    foreach (Framework.FileData fileName in Framework.FastDirectoryEnumerator.EnumerateFiles(downloadPath, "*" + Game.appID + "*.patch", SearchOption.TopDirectoryOnly))
                    {
                        newFileName = Library.Directory + @"downloading\" + fileName.Name.Replace(downloadPath, "");

                        Directory.CreateDirectory(Path.GetDirectoryName(newFileName));

                        File.Copy(fileName.Path, newFileName, true);

                        if (RemoveOld)
                            File.Delete(fileName.Path);
                    }

                    // .ACF File
                    File.Copy(Game.Library.Directory + acfName, Library.Directory + acfName, true);
                    Log(".ACF file has been created at the target directory");

                    // Remove old files?
                    if (RemoveOld)
                    {

                        if (Game.exactInstallPath != null)
                            // common
                            Directory.Delete(Game.exactInstallPath, true);

                        if (Game.downloadPath != null)
                            // downloading
                            Directory.Delete(Game.downloadPath, true);

                        // .ACF
                        File.Delete(Game.Library.Directory + acfName);

                        Log("Old files has been deleted.");
                    }

                    // Timer and Stopwatch
                    timer_TimeElapsed.Stop();
                    TimeElapsed.Stop();

                    // More Visual
                    button_Copy.Text = "Done!";
                    Log("Completed! All files successfully copied!");
                    Functions.SteamLibrary.UpdateGameLibraries();
                    Functions.Games.UpdateGamesList(Definitions.SLM.LatestSelectedGame.Library);
                }
                else
                {
                    Log("Failed");
                    System.Windows.Forms.MessageBox.Show("We don't have enough perms at the target library path, try to run as Administrator maybe?");
                }
            }
            catch (Exception ex)
            {
                timer_TimeElapsed.Stop();
                TimeElapsed.Stop();
                MessageBox.Show(ex.ToString());
            }
        }

        private void Log(string Text)
        {
            try
            {
                this.textBox_CopyLogs.AppendText(Text + "\n");
            }
            catch { }
        }

        private void timer_TimeElapsed_Tick(object sender, EventArgs e)
        {
            try
            {
                label_TimeElapsed.Text = String.Format("Time Elapsed: {0}", TimeElapsed.Elapsed);
            }
            catch { }
        }

        private void pictureBox_GameImage_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                switch (e.Button)
                {
                    default:
                        System.Diagnostics.Process.Start("http://store.steampowered.com/app/" + Game.appID.ToString() + "/");
                        break;
                    case MouseButtons.Right:
                        System.Diagnostics.Process.Start(Game.exactInstallPath);
                        break;
                }
            }
            catch { }
        }

        private void linkLabel_TargetLibrary_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Library.Directory);
            }
            catch { }
        }

        private void linkLabel_currentLibrary_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Game.Library.Directory);
            }
            catch { }
        }
    }
}
