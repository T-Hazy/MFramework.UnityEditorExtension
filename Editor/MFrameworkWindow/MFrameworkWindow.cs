using System;
using System.Collections.Generic;
using System.IO;
using MFramework.EditorExtensions;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    public class MFrameworkWindow : EditorWindowEditor
    {
        private readonly string offlinePackageDir =
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + "Unity\\";
        private GUIContent[] mainOptionMenu;
        private readonly List<OfflinePackage> offlinePackages = new List<OfflinePackage>();
        private readonly List<MFrameworkPackage> mFrameworkPackages = new List<MFrameworkPackage>();
        private readonly HorizontalTexturesGroup homePageTexturesGroup = new HorizontalTexturesGroup("Home Page");
        private TextureDrawer communityDrawer;
        private TextureDrawer githubDrawer;
        private TextureDrawer blogsDrawer;
        private TextureDrawer cloudPlatformDrawer;

        private new void OnEnable() {
            mainOptionMenu = new GUIContent[] {
                new(" Home                                              ", EditorTextures.HomePageTexture),
                new(" Package Manager                   ", EditorGUIUtility.IconContent("package_installed").image),
                new(" Editor Utility Tools                    ", EditorGUIUtility.IconContent("d_UnityLogo").image),
            };
            communityDrawer =
                new TextureDrawer(new GUIContent(" 社 区 和 学 习 平 台", EditorGUIUtility.IconContent("d_UnityLogo").image),
                    EditorTextures.CommunityTexture, this);
            githubDrawer = new TextureDrawer(new GUIContent(" Github开源库", EditorTextures.GithubIcon),
                EditorTextures.GithubTexture, this);
            blogsDrawer = new TextureDrawer(new GUIContent("云 平 台"), EditorTextures.GithubSponsorsTexture, this);
            cloudPlatformDrawer = new TextureDrawer(new GUIContent("开 发 者 大 会"), EditorTextures.CommunityTexture, this);
            //社区纹理组按钮
            URLButton unityCommunity = new URLButton(new GUIContent(EditorTextures.UnityCommunity_Icon),
                "https://developer.unity.cn/");
            URLButton fantasyCommunity = new URLButton(new GUIContent(EditorTextures.FantasyCommunityIcon),
                "https://www.fantsida.com/");
            URLButton taikrCommunity = new URLButton(new GUIContent(EditorTextures.TaikrCommunity_Icon),
                "https://www.taikr.com/");
            URLButton magicBoxCommunity = new URLButton(new GUIContent(EditorTextures.MagicBoxCommunity_Icon),
                "https://www.magesbox.com/");
            URLButton kerryCommunity = new URLButton(new GUIContent(EditorTextures.KerryTaCommunity_Icon),
                "https://www.kerryta.com/");
            communityDrawer.URLButtons.Add(unityCommunity);
            communityDrawer.URLButtons.Add(fantasyCommunity);
            communityDrawer.URLButtons.Add(taikrCommunity);
            communityDrawer.URLButtons.Add(magicBoxCommunity);
            communityDrawer.URLButtons.Add(kerryCommunity);
            homePageTexturesGroup.TextureDrawers.Add(communityDrawer);
            homePageTexturesGroup.TextureDrawers.Add(githubDrawer);
            homePageTexturesGroup.TextureDrawers.Add(blogsDrawer);
            homePageTexturesGroup.TextureDrawers.Add(cloudPlatformDrawer);
            CheckMFrameworkPackages();
            CheckOfflinePackages();
        }

        protected void OnGUI() {
            //绘制标题
            DrawRectWithOutline(new Vector2(position.width, 80), new Color(0.2f, 0.2f, 0.2f),
                new Color(0.13f, 0.13f, 0.13f), 1, () =>
                {
                    EditorGUILayout.Space(5);
                    GUILayout.Label("MFramework Manager", GUIStyleLibrary.MainTitleStyle);
                    GUILayout.Label("MFramework is a Unity3D Utility Plugin Library.", GUIStyleLibrary.SubTitleStyle);
                });
            EditorGUILayout.Space(80);
            //绘制选项菜单
            DrawVerticalGridOptionMenu("Main Menu", mainOptionMenu, 1, MainOptionMenuContent, 35, GUILayout.Width(200));
        }

        private void MainOptionMenuContent(int selectedIndex) {
            switch (selectedIndex) {
                case 0:
                    DrawHomePageContent();
                    break;
                case 1:
                    DrawPackageManagerContent();
                    break;
                case 2:
                    EditorGUILayout.BeginHorizontal();
                    DrawButton("Editor Icons Searcher", TextAlignment.Center, UnityEditorIcons.ShowInEditor,
                        GUILayout.Width(150), GUILayout.Height(30));
                    DrawButton("Editor Window Catcher", TextAlignment.Center, EditorWindowCatcher.ShowInEditor,
                        GUILayout.Width(150), GUILayout.Height(30));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    break;
            }
        }

        private void DrawHomePageContent() {
            //分页
            Vector4 padding = new Vector4(0, 0, 0, 0);
            //纹理组区域
            Vector2 textureGroupPosition = new Vector2(ResidueRect.xMin + padding.x, ResidueRect.yMin + padding.z);
            Vector2 textureGroupSize = new Vector2((position.width - textureGroupPosition.x) - padding.z,
                (position.height - textureGroupPosition.y) - padding.w);
            Rect textureGroupRect = new Rect(textureGroupPosition, textureGroupSize);
            //社区纹理组内容
            homePageTexturesGroup.DrawTexturesGroup(textureGroupRect);
        }

        private void DrawPackageManagerContent() {
            EditorGUILayout.BeginVertical();
            DrawAnimFoldedGroup("MFramework Packages", DrawMFrameworkPackagesContent, true);
            DrawAnimFoldedGroup("Local Offline Packages", DrawLocalOfflinePackagesContent);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(5);
            DrawButton("Unity Package Manager", TextAlignment.Left,
                () => { GetWindow(StringToType("UnityEditor.PackageManager.UI.PackageManagerWindow")).Show(); },
                GUILayout.Height(25));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawMFrameworkPackagesContent() {
            DrawScrollView("MFramework View", () =>
            {
                EditorGUILayout.BeginHorizontal();
                this.DrawBorderedGUIContent(new GUIContent("Index"), new Vector2(50, 25), 1,
                    new Color(0.35f, 0.35f, 0.35f));
                DrawBorderedGUIContent(new GUIContent("Package Name"), new Vector2(250, 25), 1,
                    new Color(0.35f, 0.35f, 0.35f));
                DrawBorderedGUIContent(new GUIContent("Package Github Link"), new Vector2(300, 25), 1,
                    new Color(0.35f, 0.35f, 0.35f));
                DrawBorderedGUIContent(new GUIContent("File Operation"), new Vector2(123, 25), 1,
                    new Color(0.35f, 0.35f, 0.35f));
                DrawBorderedGUIContent(new GUIContent("Package Operation"), new Vector2(155, 25), 1,
                    new Color(0.35f, 0.35f, 0.35f));
                EditorGUILayout.EndHorizontal();
                DrawScrollView("MFramework Inner View", DrawMFrameworkPackagesInfos, GUILayout.Width(910));
                EditorGUILayout.Space(10);
            }, GUILayout.ExpandWidth(true), GUILayout.Height(350));
        }

        private void DrawLocalOfflinePackagesContent() {
            DrawScrollView("Local OfflineOuter View", () =>
            {
                //DrawTable(3, 4, new Color(0.35f, 0.35f, 0.35f));
                EditorGUILayout.BeginHorizontal();
                this.DrawBorderedGUIContent(new GUIContent("Index"), new Vector2(50, 25), 1,
                    new Color(0.35f, 0.35f, 0.35f));
                DrawBorderedGUIContent(new GUIContent("Package Name"), new Vector2(250, 25), 1,
                    new Color(0.35f, 0.35f, 0.35f));
                DrawBorderedGUIContent(new GUIContent("Package Path"), new Vector2(300, 25), 1,
                    new Color(0.35f, 0.35f, 0.35f));
                DrawBorderedGUIContent(new GUIContent("File Operation"), new Vector2(123, 25), 1,
                    new Color(0.35f, 0.35f, 0.35f));
                DrawBorderedGUIContent(new GUIContent("Package Operation"), new Vector2(150, 25), 1,
                    new Color(0.35f, 0.35f, 0.35f));
                EditorGUILayout.EndHorizontal();
                DrawScrollView("Local Offline Inner View", DrawLocalOfflinePackagesInfos, GUILayout.Width(910));
                EditorGUILayout.Space(10);
            }, GUILayout.ExpandWidth(true), GUILayout.Height(350));
        }


        private void DrawLocalOfflinePackagesInfos() {
            foreach (var offlinePackage in offlinePackages) {
                EditorGUILayout.BeginVertical();
                DrawPackageInfo(offlinePackage);
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawMFrameworkPackagesInfos() {
            foreach (var package in mFrameworkPackages) {
                EditorGUILayout.BeginVertical();
                DrawPackageInfo(package);
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawBorderedGUIContent(GUIContent content, Vector2 size, int borderWidth,
            Color borderColor) {
            // 计算矩形位置
            Rect rect = GUILayoutUtility.GetRect(size.x, size.y, GUIStyleLibrary.BorderedStyle(borderWidth));
            // 绘制边框
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, borderWidth), borderColor); // Top
            EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - borderWidth, rect.width, borderWidth),
                borderColor); // Bottom
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, borderWidth, rect.height), borderColor); // Left
            EditorGUI.DrawRect(new Rect(rect.xMax - borderWidth, rect.y, borderWidth, rect.height),
                borderColor); // Right

            // 调整矩形以留出边框的空间
            rect.x += borderWidth;
            rect.y += borderWidth;
            rect.width += 2 * borderWidth;
            rect.height += 2 * borderWidth;

            // 绘制内容
            EditorGUI.LabelField(rect, content, GUIStyleLibrary.AlignmentLabelStyle(TextAnchor.MiddleCenter));
        }

        private void DrawScrollView(ref Vector2 scrollPosition, Action drawContentAction,
            params GUILayoutOption[] options) {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, options);
            drawContentAction?.Invoke();
            EditorGUILayout.EndScrollView();
        }


        private void DrawButton(string buttonName, TextAlignment alignment,
            Action action, params GUILayoutOption[] options) {
            EditorGUILayout.BeginHorizontal();
            switch (alignment) {
                case TextAlignment.Left:
                    if (GUILayout.Button(buttonName, options)) {
                        action?.Invoke();
                    }

                    GUILayout.FlexibleSpace();
                    break;
                case TextAlignment.Center:
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(buttonName, options)) {
                        action?.Invoke();
                    }

                    GUILayout.FlexibleSpace();
                    break;
                case TextAlignment.Right:
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(buttonName, options)) {
                        action?.Invoke();
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void CheckMFrameworkPackages() {
            mFrameworkPackages.Clear();
            DirectoryInfo mfPackageDirectory = new DirectoryInfo(Application.dataPath);
            var files = mfPackageDirectory.GetFiles("*.unitypackage",
                SearchOption.AllDirectories);
            for (var index = 0; index < files.Length; index++) {
                var packageInfo = files[index];
                MFrameworkPackage package = new MFrameworkPackage(packageInfo, index);
                if (package.PackageName.Contains("Unity Texture Async Loader")) {
                    package.GithubUrl = "https://github.com/T-Hazy/UnityTextureAsyncLoader";
                }

                mFrameworkPackages.Add(package);
            }
        }

        private void CheckOfflinePackages() {
            offlinePackages.Clear();
            DirectoryInfo offlinePackageDirectory = new DirectoryInfo(offlinePackageDir);
            var files = offlinePackageDirectory.GetFiles("*.unitypackage",
                SearchOption.AllDirectories);
            for (var index = 0; index < files.Length; index++) {
                var packageInfo = files[index];
                OfflinePackage offlinePackage = new OfflinePackage(packageInfo, index);
                offlinePackages.Add(offlinePackage);
            }
        }

        private void DrawPackageInfo(MFrameworkPackage package) {
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = false;
            EditorGUILayout.TextArea(package.Index.ToString(),
                GUIStyleLibrary.AlignmentTextAreaStyle(TextAnchor.MiddleCenter, true), GUILayout.Width(51),
                GUILayout.Height(50));
            EditorGUILayout.TextArea(package.PackageName,
                GUIStyleLibrary.AlignmentTextAreaStyle(TextAnchor.MiddleCenter, true), GUILayout.Width(251),
                GUILayout.Height(50));
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextArea(package.GithubUrl,
                GUIStyleLibrary.AlignmentTextAreaStyle(TextAnchor.MiddleCenter, true), GUILayout.Width(245),
                GUILayout.Height(50));
            GUI.enabled = true;
            DrawButton("Open", TextAlignment.Center, () =>
            {
                if (package.GithubUrl.Contains("github.com")) Application.OpenURL(package.GithubUrl);
            }, GUILayout.Height(50));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginVertical();
            DrawButton("Copy To Desktop", TextAlignment.Left, package.CopyToDesktop, GUILayout.Width(120),
                GUILayout.Height(25));
            DrawButton("Navigate Package", TextAlignment.Left, package.NavigateToPackage, GUILayout.Width(120),
                GUILayout.Height(25));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Show Import Panel", GUILayout.Width(115)); // 控制标签的宽度
            package.ShowImportPanel =
                EditorGUILayout.Toggle(package.ShowImportPanel, GUILayout.Width(20)); // 控制复选框的宽度
            EditorGUILayout.EndHorizontal();
            DrawButton("Import", TextAlignment.Left, package.Import, GUILayout.Width(145),
                GUILayout.Height(25));
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawPackageInfo(OfflinePackage package) {
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = false;
            EditorGUILayout.TextArea(package.index.ToString(),
                GUIStyleLibrary.AlignmentTextAreaStyle(TextAnchor.MiddleCenter, true), GUILayout.Width(51),
                GUILayout.Height(50));
            EditorGUILayout.TextArea(package.packageName,
                GUIStyleLibrary.AlignmentTextAreaStyle(TextAnchor.MiddleCenter, true), GUILayout.Width(251),
                GUILayout.Height(50));
            EditorGUILayout.TextArea(package.packagePath,
                GUIStyleLibrary.AlignmentTextAreaStyle(TextAnchor.MiddleCenter, true), GUILayout.Width(301),
                GUILayout.Height(50));
            GUI.enabled = true;

            EditorGUILayout.BeginVertical();
            DrawButton("Copy To Desktop", TextAlignment.Left, package.CopyToDesktop, GUILayout.Width(125),
                GUILayout.Height(25));
            DrawButton("Open Folder", TextAlignment.Left, package.OpenFolder, GUILayout.Width(125),
                GUILayout.Height(25));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Show Import Panel", GUILayout.Width(120)); // 控制标签的宽度
            package.ShowImportPanel =
                EditorGUILayout.Toggle(package.ShowImportPanel, GUILayout.Width(20)); // 控制复选框的宽度
            EditorGUILayout.EndHorizontal();
            DrawButton("Import", TextAlignment.Left, package.Import, GUILayout.Width(150),
                GUILayout.Height(25));
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }


        [MenuItem("Assets/MFramework", false, -1000)]
        public static void OpenGameManagerWindow() {
            var window = GetWindow<MFrameworkWindow>();
            window.position = new Rect(
                new(Screen.currentResolution.width / 6, Screen.currentResolution.height / 5),
                new(Screen.currentResolution.width / 2, Screen.currentResolution.height / 2));
            window.minSize = new Vector2(1150, 300);
            window.Show();
        }
    }
}