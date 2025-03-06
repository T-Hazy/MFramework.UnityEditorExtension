using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace MFramework.EditorExtensions
{
    public class AnimFoldGroup
    {
        public string GroupName;
        public GUIContent FoldContent;
        public GUIContent UnfoldContent;
        public AnimBool AnimBool;
    }

    public class EditorScrollView
    {
        public string ScrollViewName;
        public Vector2 ScrollViewPosition;
    }

    public class GridOptionMenu
    {
        public int selectedIndex = 0;
        public GUIContent[] options;
        public int xCount = 1;
        public List<GUILayoutOption> layoutOptions;
    }

    public class HorizontalTexturesGroup
    {
        public string GroupName;
        public List<TextureDrawer> TextureDrawers = new List<TextureDrawer>();

        public void DrawGroupBackground(Rect groupRect, Color rectColor, Color outlineColor, int thickness = 1)
        {
            EditorGUI.DrawRect(groupRect, rectColor);
            groupRect.DrawOutline(outlineColor, thickness);
        }

        public void DrawTexturesGroup(Rect groupRect, int space = 2)
        {
            int drawerWidth = (int)(groupRect.width / TextureDrawers.Count);
            for (var index = 0; index < TextureDrawers.Count; index++)
            {
                var drawer = TextureDrawers[index];
                Vector2 drawerPosition = new Vector2(groupRect.xMin + drawerWidth * index, groupRect.yMin);
                Vector2 drawerSize = new Vector2(drawerWidth, groupRect.height);
                Rect drawerRect = new Rect(drawerPosition, drawerSize);
                drawer.Draw(drawerRect, index);
            }
        }

        private void Draw(Rect rect, int space)
        {
        }

        public HorizontalTexturesGroup(string groupName)
        {
            GroupName = groupName;
        }
    }

    public class TextureDrawer
    {
        public GUIContent DrawerTitle;
        public Texture Texture;
        public ScaleMode ScaleMode = ScaleMode.ScaleAndCrop;
        public List<URLButton> URLButtons = new List<URLButton>();

        private AnimBool animBool;

        public TextureDrawer(GUIContent drawerTitle, Texture texture, EditorWindow window,
            ScaleMode scaleMode = ScaleMode.ScaleAndCrop)
        {
            animBool = new AnimBool(false);
            animBool.valueChanged.AddListener(window.Repaint);
            Texture = texture;
            ScaleMode = scaleMode;
            DrawerTitle = drawerTitle;
        }

        public void Draw(Rect rect, int index)
        {
            // 更新 waistHeight 的动画值
            var waistHeight = Mathf.Lerp(rect.height * 0.12f, rect.height * 0.85f, animBool.faded);
            Vector2 texturePosition = new Vector2(rect.xMin, rect.yMin);
            Vector2 textureSize = new Vector2(rect.width, rect.height - waistHeight);
            Rect textureAreaRect = new Rect(texturePosition, textureSize);
            GUI.DrawTexture(textureAreaRect, Texture, ScaleMode);
            //var maskColor = index % 2 == 0 ? ColourLibrary.GrayPercentage(0f, 0.5f) : Color.clear;
            //EditorGUI.DrawRect(textureAreaRect, maskColor);
            // 底部区域绘制
            Vector2 bottomAreaPosition = new Vector2(textureAreaRect.xMin, textureAreaRect.yMax);
            Vector2 bottomAreaAreaSize = new Vector2(textureAreaRect.width, rect.height - textureAreaRect.height);
            Rect buttonAreaRect = new Rect(bottomAreaPosition, bottomAreaAreaSize);
            Color bottomAreaColor =
                index % 2 == 0 ? new Color(0.2f, 0.2f, 0.2f) : new Color(0.25f, 0.25f, 0.25f);
            EditorGUI.DrawRect(buttonAreaRect, bottomAreaColor);
            if (animBool.target)
            {
                for (var i = 0; i < URLButtons.Count; i++)
                {
                    Vector2 buttonRectPosition =
                        new Vector2(buttonAreaRect.xMin + 10, buttonAreaRect.yMin + i * 40 + 10);
                    Vector2 buttonRectSize = new Vector2(buttonAreaRect.width - 20, 30);
                    Rect buttonRect = new Rect(buttonRectPosition, buttonRectSize);
                    var urlButton = URLButtons[i];
                    if (GUI.Button(buttonRect, urlButton.ButtonContent))
                    {
                        urlButton.Invoke();
                    }
                }

                Vector2 backPosition = new Vector2(buttonAreaRect.xMin + 10, buttonAreaRect.yMax - 50);
                Vector2 backSize = new Vector2(buttonAreaRect.width - 200, 25);
                Rect backRect = new Rect(backPosition, backSize);
                if (GUI.Button(backRect, "< Back"))
                {
                    animBool.target = !animBool.target;
                }
            }
            else
            {
                // 展开按钮绘制
                Vector2 buttonRectPosition = new Vector2(buttonAreaRect.xMin + 10, buttonAreaRect.yMin + 20);
                Vector2 buttonRectSize = new Vector2(buttonAreaRect.width - 20, 40);
                Rect buttonRect = new Rect(buttonRectPosition, buttonRectSize);
                if (GUI.Button(buttonRect, DrawerTitle))
                {
                    animBool.target = !animBool.target;
                }
            }
        }
    }

    public class URLButton
    {
        public GUIContent ButtonContent;
        public string URL;

        public URLButton(GUIContent buttonContent, string url)
        {
            ButtonContent = buttonContent;
            URL = url;
        }

        public void Invoke() => Application.OpenURL(URL);
    }

    public class EditorWindowEditor : EditorWindow
    {
        private GUIStyle defaultAnimCollapseSectionStyle;

        private readonly Dictionary<string, AnimationCollapseGroup> groups =
            new Dictionary<string, AnimationCollapseGroup>();

        //FIX 使用折叠组名称字符串存储，可能无法绘制两个相同名称的折叠组。
        private readonly Dictionary<string, AnimFoldGroup> animFoldedGroups = new Dictionary<string, AnimFoldGroup>();

        //FIX 使用折叠组名称字符串存储，可能无法绘制两个相同名称的视图组。
        private readonly Dictionary<string, EditorScrollView> scrollViews = new Dictionary<string, EditorScrollView>();

        //FIX 使用折叠组名称字符串存储，可能无法绘制两个相同名称的视图组。
        private readonly Dictionary<string, GridOptionMenu> verticalGridOptionMenus =
            new Dictionary<string, GridOptionMenu>();

        private readonly Dictionary<string, HorizontalTexturesGroup> horizontalTexturesGroup =
            new Dictionary<string, HorizontalTexturesGroup>();

        protected void DrawTable(int horizontalLength, int verticalLength, Color borderColor)
        {
            for (int i = 0; i < horizontalLength; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < verticalLength; j++)
                {
                    EditorGUILayout.BeginVertical();
                    DrawTableCell(new GUIContent($"{i + 1},{j + 1}"), TextAnchor.MiddleCenter, 2, borderColor);
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawTableCell(GUIContent content, TextAnchor textAnchor, int borderWidth, Color borderColor)
        {
            GUIStyle labelStyle = GUIStyleLibrary.AlignmentLabelStyle(textAnchor);
            Rect textRect = GUILayoutUtility.GetRect(content, labelStyle);
            Rect borderRect = new Rect(textRect.x, textRect.y, textRect.width + 2 * borderWidth,
                textRect.height + 2 * borderWidth);
            EditorGUI.DrawRect(new Rect(borderRect.x, borderRect.y, borderRect.width, borderWidth), borderColor); // Top
            EditorGUI.DrawRect(new Rect(borderRect.x, borderRect.yMax - borderWidth, borderRect.width, borderWidth),
                borderColor); // Bottom
            EditorGUI.DrawRect(new Rect(borderRect.x, borderRect.y, borderWidth, borderRect.height),
                borderColor); // Left
            EditorGUI.DrawRect(new Rect(borderRect.xMax - borderWidth, borderRect.y, borderWidth, borderRect.height),
                borderColor); // Right


            // 绘制内容
            EditorGUI.LabelField(textRect, content, labelStyle);
        }

        private void DrawBorderedGUIContent(GUIContent content, Vector2 size, int borderWidth, Color borderColor)
        {
            var labelStyle = GUIStyleLibrary.AlignmentLabelStyle(TextAnchor.MiddleCenter);

            // 计算文本内容的矩形位置
            var textRect = new Rect(Vector2.zero, size); // 使用传入的 size

            // 计算边框的矩形位置，包括填充
            var borderRect = new Rect(textRect.x - borderWidth, textRect.y - borderWidth,
                textRect.width + 2 * borderWidth, textRect.height + 2 * borderWidth);

            // 绘制边框
            EditorGUI.DrawRect(new Rect(borderRect.x, borderRect.y, borderRect.width, borderWidth), borderColor); // Top
            EditorGUI.DrawRect(new Rect(borderRect.x, borderRect.yMax - borderWidth, borderRect.width, borderWidth),
                borderColor); // Bottom
            EditorGUI.DrawRect(new Rect(borderRect.x, borderRect.y, borderWidth, borderRect.height),
                borderColor); // Left
            EditorGUI.DrawRect(new Rect(borderRect.xMax - borderWidth, borderRect.y, borderWidth, borderRect.height),
                borderColor); // Right

            // 绘制内容
            EditorGUI.LabelField(textRect, content, labelStyle);
        }


        protected void DrawVerticalSeparator(float separatorWidth, Color color)
        {
            Rect rect = GUILayoutUtility.GetLastRect(); // 获取左边菜单的Rect
            rect.x += rect.width; // 调整竖线的位置
            rect.width = separatorWidth; // 设置竖线的宽度
            rect.height = maxSize.y;
            EditorGUI.DrawRect(rect, color); // 使用EditorGUI.DrawRect绘制背景
        }

        protected void DrawHorizontalSeparator(float separatorHeight, Color color)
        {
            Rect rect = GUILayoutUtility.GetLastRect(); // 获取上一个控件的Rect
            rect.y += rect.height; // 调整横线的位置
            rect.height = separatorHeight; // 设置横线的高度
            rect.width = maxSize.x;
            EditorGUI.DrawRect(rect, color); // 使用EditorGUI.DrawRect绘制横线
        }


        protected void DrawAnimCollapseSection(string label, Action drawMethod, Color color,
            UnityEditor.Editor editor, int indent = 0, bool showOnlyInEditorMode = true,
            bool isDefaultExpanded = false)
        {
            if (groups.TryGetValue(label, out AnimationCollapseGroup section))
            {
                AnimationCollapseGroup.Show(section, indent);
            }
            else
            {
                AnimationCollapseGroup newSection = new AnimationCollapseGroup(label, showOnlyInEditorMode,
                    isDefaultExpanded,
                    drawMethod, editor, color);
                AnimationCollapseGroup.Show(newSection, indent);
                groups.Add(label, newSection);
            }
        }

        protected Rect DrawRectWithOutline(Vector2 rectPosition, Vector2 rectSize, Color fillColor, Color outlineColor,
            int outlineWidth = 1, Action drawMethod = null)
        {
            Rect rect = new Rect(rectPosition, rectSize);
            EditorGUI.DrawRect(rect, fillColor);
            rect.DrawOutline(outlineColor, outlineWidth);
            GUILayout.BeginArea(rect);
            drawMethod?.Invoke();
            GUILayout.EndArea();
            return rect;
        }

        protected Rect DrawRectWithOutline(Vector2 rectSize, Color fillColor, Color outlineColor, int outlineWidth = 1,
            Action drawMethod = null)
        {
            Rect rect = new Rect(Vector2.zero, rectSize);
            EditorGUI.DrawRect(rect, fillColor);
            rect.DrawOutline(outlineColor, outlineWidth);
            GUILayout.BeginArea(rect);
            drawMethod?.Invoke();
            GUILayout.EndArea();
            return rect;
        }


        protected void DrawHorizontalTexturesGroup(string groupName, List<TextureDrawer> textureDrawers,
            Vector4 padding)
        {
            // if (!horizontalTexturesGroup.TryGetValue(groupName, out HorizontalTexturesGroup textureGroup)) {
            //     Vector2 textureGroupPosition = new Vector2(ResidueRect.xMin + padding.x, ResidueRect.yMin + padding.z);
            //     Vector2 textureGroupSize = new Vector2((position.width - textureGroupPosition.x) - padding.z,
            //         (position.height - textureGroupPosition.y) - padding.w);
            //     Rect textureGroupRect = new Rect(textureGroupPosition, textureGroupSize);
            //     textureGroup = new HorizontalTexturesGroup(groupName, textureGroupRect);
            //     horizontalTexturesGroup.Add(groupName, textureGroup);
            // }
            //
            // textureGroup.DrawGroupBackground(ColourLibrary.GrayPercentage(0.2f), Color.clear);
            // textureGroup.DrawTexturesGroup();
            // int horizontalPadding = (int)(padding.x + padding.y);
            // int verticalPadding = (int)(padding.z + padding.w);
            // // 计算每个Texture的宽度和高度- space * textures.Length - 1
            // int textureButtonDrawerWidth = (int)(backgroundRect.width / textures.Length) + space;
            // TextureButtonDrawer drawer = new TextureButtonDrawer();
            // for (var i = 0; i < textures.Length; i++) {
            //     //纹理绘制区域
            //     Vector2 texturePosition =
            //         new Vector2(backgroundRect.xMin + (horizontalPageWidth * i), backgroundRect.yMin);
            //     Vector2 horizontalPageSize =
            //         new Vector2(horizontalPageWidth, backgroundRect.height - interactionRectHeight);
            //     Rect textureAreaRect = new Rect(texturePosition, horizontalPageSize);
            //     Color pageColor = i % 2 == 0 ? ColourLibrary.GrayPercentage(0.4f) : ColourLibrary.GrayPercentage(0.6f);
            //     EditorGUI.DrawRect(textureAreaRect, pageColor);
            //     GUI.DrawTexture(textureAreaRect, textures[i], scaleMode);
            //     //绘制功能区域
            //     Vector2 functionAreaPosition = new Vector2(textureAreaRect.xMin, textureAreaRect.yMax);
            //     Vector2 functionAreaSize =
            //         new Vector2(textureAreaRect.width, backgroundRect.height - textureAreaRect.height);
            //     Rect interactionAreaRect = new Rect(functionAreaPosition, functionAreaSize);
            //     Color functionAreaColor =
            //         i % 2 == 0 ? ColourLibrary.GrayPercentage(0.2f) : ColourLibrary.GrayPercentage(0.3f);
            //     EditorGUI.DrawRect(interactionAreaRect, functionAreaColor);
            //     if (GUI.Button(interactionAreaRect, textures[i])) {
            //     }
            // }
        }


        protected Rect ResidueRect
        {
            get
            {
                var lastRect = GUILayoutUtility.GetLastRect();
                var residueRect = new Rect(lastRect.xMax, lastRect.yMin, this.position.width - lastRect.xMax,
                    this.position.height - lastRect.yMin);
                return residueRect;
            }
        }

        protected void DrawVerticalGridOptionMenu(string menuTitle, GUIContent[] menuItems, int xCount,
            Action<int> drawMethod, int selectedHeight = 35, params GUILayoutOption[] options)
        {
            if (!verticalGridOptionMenus.TryGetValue(menuTitle, out GridOptionMenu optionMenu))
            {
                optionMenu = new GridOptionMenu
                {
                    selectedIndex = 0,
                    xCount = xCount,
                    options = menuItems,
                    layoutOptions = options.ToList()
                };
                optionMenu.layoutOptions.Add(GUILayout.Height(menuItems.Length * selectedHeight));
                verticalGridOptionMenus.Add(menuTitle, optionMenu);
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(200));
            optionMenu.selectedIndex = GUILayout.SelectionGrid(optionMenu.selectedIndex, optionMenu.options,
                optionMenu.xCount, optionMenu.layoutOptions.ToArray());
            EditorGUILayout.EndVertical();
            EditorGUI.DrawRect(ResidueRect, new Color(0.25f, 0.25f, 0.25f));
            drawMethod?.Invoke(optionMenu.selectedIndex);
            EditorGUILayout.EndHorizontal();
        }

        protected void DrawScrollView(string viewTitle, Action drawContentAction, params GUILayoutOption[] options)
        {
            if (!scrollViews.TryGetValue(viewTitle, out EditorScrollView scrollView))
            {
                scrollView = new EditorScrollView()
                {
                    ScrollViewName = viewTitle,
                    ScrollViewPosition = Vector2.zero
                };
                scrollViews.Add(viewTitle, scrollView);
            }

            scrollView.ScrollViewPosition = EditorGUILayout.BeginScrollView(scrollView.ScrollViewPosition, options);
            drawContentAction?.Invoke();
            EditorGUILayout.EndScrollView();
        }

        protected void DrawAnimFoldedGroup(string foldedTitle, Action drawContentAction, bool expand = false)
        {
            // 尝试从字典中获取 AnimFoldGroup 对象
            if (!animFoldedGroups.TryGetValue(foldedTitle, out AnimFoldGroup foldGroup))
            {
                // 如果字典中不存在，创建一个新的 AnimFoldGroup 对象并添加到字典中
                foldGroup = new AnimFoldGroup()
                {
                    GroupName = foldedTitle,
                    FoldContent = new GUIContent("▶ " + foldedTitle),
                    UnfoldContent = new GUIContent("▼ " + foldedTitle),
                    AnimBool = new AnimBool(expand)
                };
                foldGroup.AnimBool.valueChanged.AddListener(Repaint);
                animFoldedGroups.Add(foldedTitle, foldGroup);
            }

            // 绘制动画折叠组
            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(foldGroup.AnimBool.target ? foldGroup.UnfoldContent : foldGroup.FoldContent,
                    GUIStyleLibrary.AnimFadeGroupButtonStyle))
            {
                // 关闭所有其他折叠组
                foreach (var group in animFoldedGroups.Values)
                {
                    if (group != foldGroup)
                    {
                        group.AnimBool.target = false;
                    }
                }

                // 切换当前折叠组的状态
                foldGroup.AnimBool.target = !foldGroup.AnimBool.target;
            }

            GUILayout.EndHorizontal();

            // 开始绘制动画折叠内容
            if (EditorGUILayout.BeginFadeGroup(foldGroup.AnimBool.faded))
            {
                EditorGUILayout.Space(5);
                drawContentAction?.Invoke();
            }

            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.EndVertical();
        }

        protected static Type StringToType(string typeName)
        {
            typeName ??= string.Empty;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var type = assembly.GetType(typeName);
                if (type != null) return type;
            }

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase))
                    {
                        return type;
                    }
                }
            }

            return null;
        }

        protected void OnEnable()
        {
            FixEditorBug();
        }

        protected void OnDisable()
        {
            groups.Clear();
            animFoldedGroups.Clear();
        }

        private void FixEditorBug()
        {
            var remainingBuggedEditors = FindObjectsOfType<EditorWindowEditor>();
            foreach (var editor in remainingBuggedEditors)
            {
                if (editor == this)
                {
                    continue;
                }

                DestroyImmediate(editor);
            }
        }
    }
}