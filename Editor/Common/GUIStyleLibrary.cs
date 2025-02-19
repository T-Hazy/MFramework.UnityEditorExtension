using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MFramework.UtilityLibrary
{
    public static class GUIStyleLibrary
    {
        #region GUIStyle

        /// <summary>
        /// 平面按钮
        /// </summary>
        public static GUIStyle flatButton { get; private set; }

        /// <summary>
        /// 弹出式平面按钮
        /// </summary>
        public static GUIStyle popupFlatButton;

        /// <summary>
        /// 弹出样式
        /// </summary>
        public static GUIStyle popup;

        /// <summary>
        /// 黑暗窗口
        /// </summary>
        public static GUIStyle darkWindow;

        /// <summary>
        /// 左框对齐
        /// </summary>
        public static GUIStyle alignLeftBox;

        /// <summary>
        /// 警告标签
        /// </summary>
        public static GUIStyle warningLabel;

        /// <summary>
        /// 警告标签无样式
        /// </summary>
        public static GUIStyle warningLabelNoStyle;

        public static GUIStyle labelToggle;

        private static Texture2D flatButtonTexture;
        private static Texture2D popupTex;
        private static Texture2D darkWindowTexNormal;
        private static Texture2D darkWindowTexOnNormal;
        private static readonly List<Texture2D> texture2Ds = new List<Texture2D>();

        #endregion

        static GUIStyleLibrary()
        {
            CreateFlatButton();
            CreatePopupFlatButton();
            CreatePopup();
            CreateDarkWindow();
            CreateAlignLeftBox();
            CreateWarningLabel();
            CreateWarningLabelNoStyle();
        }


        public static void FontSizeUp()
        {
        }

        public static void FontSizeDown()
        {
        }

        private static void SetGUIStyleFontSize()
        {
        }

        private static GUIStyle CreateLabelToggle(Color onColor)
        {
            var style = new GUIStyle(GUI.skin.button);
            style.alignment = TextAnchor.MiddleLeft;
            //style.border = new RectOffset(0, 0, 1, underLine + 1);
            style.border = new RectOffset(4, 1, 0, 0);

            var bgColorHover = Vector4.one * 0.5f;
            var bgColorActive = Vector4.one * 0.7f;

            texture2Ds.Add(style.onNormal.background = CreateToggleOnTexture(onColor, Color.clear));
            texture2Ds.Add(style.onHover.background = CreateToggleOnTexture(onColor, bgColorHover));
            texture2Ds.Add(style.onActive.background = CreateToggleOnTexture(onColor * 1.5f, bgColorActive));
            
            texture2Ds.Add(style.normal.background = CreateTexture(Color.clear));
            texture2Ds.Add(style.hover.background = CreateTexture(bgColorHover));
            texture2Ds.Add(style.active.background = CreateTexture(bgColorActive));

            return style;
        }

        private static Texture2D CreateToggleOnTexture(Color col, Color bg)
        {
            var tex = new Texture2D(6, 1);

            for (var x = 0; x < tex.width; ++x)
            {
                var c = (x < 3) ? col : bg;
                for (var y = 0; y < tex.height; ++y)
                {
                    tex.SetPixel(x, y, c);
                }
            }

            tex.Apply();

            return tex;
        }

        private static Texture2D CreateTexture(Color col)
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, col);
            tex.Apply();

            return tex;
        }

        private static void CreateFlatButton()
        {
            var style = new GUIStyle(GUI.skin.label)
            {
                wordWrap = false,
                alignment = TextAnchor.MiddleCenter
            };

            var toggle = GUI.skin.toggle;
            style.normal.textColor = toggle.normal.textColor;
            style.hover.textColor = toggle.hover.textColor;

            flatButtonTexture = new Texture2D(1, 1);
            flatButtonTexture.SetPixels(new[] { new Color(0.5f, 0.5f, 0.5f, 0.5f) });
            flatButtonTexture.Apply();
            style.hover.background = flatButtonTexture;

            style.name = nameof(flatButton);
            flatButton = style;
        }


        private static void CreatePopupFlatButton()
        {
            var style = new GUIStyle(flatButton)
            {
                alignment = GUI.skin.label.alignment,
                padding = new RectOffset(24, 48, 2, 2),
                name = nameof(popupFlatButton)
            };

            popupFlatButton = style;
        }

        private static void CreatePopup()
        {
            var style = new GUIStyle(GUI.skin.box)
            {
                border = new RectOffset()
            };

            popupTex = new Texture2D(1, 1);
            var brightness = 0.2f;
            var alpha = 0.9f;
            popupTex.SetPixels(new[] { new Color(brightness, brightness, brightness, alpha) });
            popupTex.Apply();

            style.normal.background =
                style.hover.background = popupTex;

            style.name = nameof(popup);
            popup = style;
        }


        private static void CreateDarkWindow()
        {
            var style = new GUIStyle(GUI.skin.window);

            style.normal.background = darkWindowTexNormal = CreateTexDark(style.normal.background, 0.5f, 1.4f);
            style.onNormal.background = darkWindowTexOnNormal = CreateTexDark(style.onNormal.background, 0.6f, 1.5f);

            style.name = nameof(darkWindow);

            darkWindow = style;
        }

        private static void CreateAlignLeftBox()
        {
            var style = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.MiddleLeft,
                name = nameof(alignLeftBox)
            };

            alignLeftBox = style;
        }

        private static Texture2D CreateTexDark(Texture2D src, float colorRate, float alphaRate)
        {
            // copy texture trick.
            // Graphics.CopyTexture(src, dst) must same format src and dst.
            // but src format can't call GetPixels().
            var tmp = RenderTexture.GetTemporary(src.width, src.height);
            Graphics.Blit(src, tmp);

            var prev = RenderTexture.active;
            RenderTexture.active = prev;

            var dst = new Texture2D(src.width, src.height, TextureFormat.RGBA32, false);
            dst.ReadPixels(new Rect(0f, 0f, src.width, src.height), 0, 0);


            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(tmp);


            var pixels = dst.GetPixels();
            for (var i = 0; i < pixels.Length; ++i)
            {
                var col = pixels[i];
                col.r *= colorRate;
                col.g *= colorRate;
                col.b *= colorRate;
                col.a *= alphaRate;

                pixels[i] = col;
            }

            dst.SetPixels(pixels);
            dst.Apply();

            return dst;
        }


        private static void CreateWarningLabel()
        {
            var style = new GUIStyle(GUI.skin.box)
            {
                alignment = GUI.skin.label.alignment,
                richText = true,
                name = nameof(warningLabel)
            };

            warningLabel = style;
        }

        private static void CreateWarningLabelNoStyle()
        {
            var style = new GUIStyle(GUI.skin.label)
            {
                richText = true,
                name = nameof(warningLabelNoStyle)
            };

            warningLabelNoStyle = style;
        }


        public static GUIStyle BoxSkinStyle => new GUIStyle(GUI.skin.box);

        public static GUIStyle AlignmentLabelStyle(TextAnchor anchor)
        {
            var centeredLabelStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = anchor
            };
            return centeredLabelStyle;
        }

        public static GUIStyle AlignmentTextAreaStyle(TextAnchor anchor, bool wordWrap)
        {
            var centeredLabelStyle = new GUIStyle(EditorStyles.textArea)
            {
                alignment = anchor, wordWrap = wordWrap
            };
            return centeredLabelStyle;
        }

        public static GUIStyle MainTitleStyle { get; } = new()
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 25,
            fontStyle = FontStyle.Bold,
            normal = new GUIStyleState()
            {
                textColor = Color.white
            }
        };

        public static GUIStyle SubTitleStyle { get; } = new()
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 12,
            fontStyle = FontStyle.Normal,
            normal = new GUIStyleState()
            {
                textColor = new Color(0.5f, 0.5f, 0.5f)
            }
        };

        public static GUIStyle AnimFadeGroupButtonStyle
        {
            get
            {
                GUIStyle style = new GUIStyle(GUI.skin.button);
                style.alignment = TextAnchor.MiddleLeft;
                style.normal.textColor = new Color(0.9f, 0.9f, 0.9f); // 正常状态下的文本颜色为灰色
                style.hover.textColor = Color.white; // 鼠标悬停时的文本颜色为白色
                style.active.textColor = Color.white; // 按钮被点击时的文本颜色为白色
                style.normal.background = EditorGUIUtility.whiteTexture; // 正常状态下的背景色为白色
                style.hover.textColor = Color.white;
                return style;
            }
        }

        public static GUIStyle BorderedStyle(int borderSize)
        {
            var borderedStyle = new GUIStyle(GUI.skin.box);
            borderedStyle.fontStyle = FontStyle.Bold;
            borderedStyle.border = new RectOffset(borderSize, borderSize, borderSize, borderSize); // 设置边框大小
            return borderedStyle;
        }
    }
}