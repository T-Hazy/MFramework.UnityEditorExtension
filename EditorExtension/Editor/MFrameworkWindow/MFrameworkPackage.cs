using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using Debug = UnityEngine.Debug;

public class MFrameworkPackage
{
    public int Index { get; private set; }
    public string PackageName { get; private set; }
    public string PackagePath { get; private set; }
    public string PackageVersions { get; private set; }
    public FileInfo PackageFileInfo { get; private set; }
    public string GithubUrl = "Not Link";

    private string relativePath => $"Assets/MFramework/Packages/{PackageName}.unitypackage";

    public MFrameworkPackage(FileInfo fileInfo, int index) {
        PackageFileInfo = fileInfo;
        PackagePath = fileInfo.FullName;
        PackageName = fileInfo.Name.Replace(".unitypackage", "");
        try {
            var temp = fileInfo.FullName.Substring(fileInfo.FullName.IndexOf("v"));
            PackageVersions = temp.Substring(0, temp.IndexOf("."));
        }
        catch (Exception) {
            PackageVersions = "Unknown";
        }

        Index = index;
    }

    public void CopyToDesktop() {
        File.Copy(PackagePath,
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + $"\\{PackageName}.unitypackage", true);
        Debug.Log("The file has been copied to the desktop");
    }

    public void NavigateToPackage() {
        UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(relativePath);
        if (asset != null) {
            Selection.activeObject = asset;
            EditorApplication.delayCall += () =>
            {
                EditorUtility.FocusProjectWindow();
                EditorGUIUtility.PingObject(asset);
            };
        }
        else {
            Debug.LogWarning("Asset not found at path: " + relativePath);
        }
    }

    public bool ShowImportPanel { get; set; } = true;

    public void Import() {
        AssetDatabase.ImportPackage(PackagePath, ShowImportPanel);
    }
}