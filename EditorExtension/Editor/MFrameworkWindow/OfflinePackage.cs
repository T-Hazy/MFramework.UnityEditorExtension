using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using Debug = UnityEngine.Debug;

public class OfflinePackage
{
    public int Index { get; private set; }
    public string PackageName { get; private set; }
    public string PackagePath { get; private set; }
    public FileInfo PackageFileInfo { get; private set; }

    public OfflinePackage(FileInfo fileInfo, int index) {
        PackageFileInfo = fileInfo;
        PackageName = fileInfo.Name.Replace(".unitypackage", "");
        PackagePath = fileInfo.FullName;
        Index = index;
    }

    public void CopyToDesktop() {
        File.Copy(PackagePath,
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + $"\\{PackageName}.unitypackage", true);
        Debug.Log("The file has been copied to the desktop");
    }

    public void OpenFolder() {
        Process.Start("explorer.exe", "/select," + PackagePath);
    }

    public bool ShowImportPanel { get; set; } = true;

    public void Import() {
        AssetDatabase.ImportPackage(PackagePath, ShowImportPanel);
    }
}