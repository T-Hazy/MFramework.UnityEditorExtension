using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace MFramework.EditorExtensions
{
    public class OfflinePackage
    {
        public int index { get; private set; }
        public string packageName { get; private set; }
        public string packagePath { get; private set; }
        public FileInfo PackageFileInfo { get; private set; }

        public OfflinePackage(FileInfo fileInfo, int index)
        {
            PackageFileInfo = fileInfo;
            packageName = fileInfo.Name.Replace(".unitypackage", "");
            packagePath = fileInfo.FullName;
            this.index = index;
        }

        public void CopyToDesktop()
        {
            File.Copy(packagePath,
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + $"\\{packageName}.unitypackage", true);
            Debug.Log("The file has been copied to the desktop");
        }

        public void OpenFolder()
        {
            Process.Start("explorer.exe", "/select," + packagePath);
        }

        public bool ShowImportPanel { get; set; } = true;

        public void Import()
        {
            AssetDatabase.ImportPackage(packagePath, ShowImportPanel);
        }
    }
}