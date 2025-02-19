using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable RedundantNameQualifier

namespace MFramework.EditorExtension
{
    internal class AnimationCollapseGroup
    {
        private const string SettingsPrefix = "MFramework-EditorExtension-";
        private const float collapseSpeed = 2f;
        private static GUIStyle collapsableSectionStyle;
        private static GUIStyle buttonFoldoutStyle;
        private static GUIStyle helpBoxNoPaddingStyle;
        private readonly UnityEditor.AnimatedValues.AnimBool animBool;
        private readonly System.Action drawAction;
        private readonly List<AnimationCollapseGroup> subAnimationCollapseGroups = new List<AnimationCollapseGroup>();
        public void Invoke() => drawAction?.Invoke();

        public bool isExpanded
        {
            get => animBool.target;
            set
            {
                if (animBool.target == value) return;
                animBool.target = value;
                if (value) CollapseSiblings();
            }
        }

        public float faded => animBool.faded;
        public GUIContent Label { get; private set; }
        public bool showOnlyInEditMode { get; private set; }
        public Color backgroundColor { get; private set; }

        private string saveName => (SettingsPrefix + "Expand-" + Label.text);

        public AnimationCollapseGroup(string label, bool showOnlyInEditMode, bool isDefaultExpanded,
            System.Action action,
            UnityEditor.Editor editor, Color backgroundColor, List<AnimationCollapseGroup> groupItems = null)
            : this(new GUIContent(label), showOnlyInEditMode, isDefaultExpanded, action, editor, backgroundColor,
                groupItems)
        {
        }

        public AnimationCollapseGroup(GUIContent label, bool showOnlyInEditMode, bool isDefaultExpanded,
            System.Action action, UnityEditor.Editor editor, Color backgroundColor,
            List<AnimationCollapseGroup> groupItems = null)
        {
            Label = label;
            Label.text = " " + Label.text;
            this.showOnlyInEditMode = showOnlyInEditMode;
            this.drawAction = action;
            isDefaultExpanded = EditorPrefs.GetBool(saveName, isDefaultExpanded);
            this.backgroundColor = backgroundColor;
            subAnimationCollapseGroups = groupItems;
            animBool = new UnityEditor.AnimatedValues.AnimBool(isDefaultExpanded)
            {
                speed = collapseSpeed
            };
            animBool.valueChanged.AddListener(editor.Repaint);
        }

        public void Save()
        {
            EditorPrefs.SetBool(saveName, isExpanded);
        }

        private void CollapseSiblings()
        {
            // 确保只有一个动画组处于展开状态
            if (subAnimationCollapseGroups == null) return;
            foreach (var subGroup in subAnimationCollapseGroups.Where(subGroup =>
                         subGroup != this && subGroup.isExpanded))
            {
                subGroup.isExpanded = false;
            }
        }

        internal static void CreateStyles()
        {
            collapsableSectionStyle ??= new GUIStyle(GUI.skin.box)
            {
                padding =
                {
                    top = 0,
                    bottom = 0
                }
            };

            buttonFoldoutStyle ??= new GUIStyle(EditorStyles.foldout)
            {
                margin = new RectOffset(),
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };

            if (helpBoxNoPaddingStyle != null) return;
            helpBoxNoPaddingStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(),
                //_styleHelpBoxNoPad.border = new RectOffset();
                overflow = new RectOffset(),
                margin = new RectOffset()
            };
            helpBoxNoPaddingStyle.margin = new RectOffset(8, 0, 0, 0);
            helpBoxNoPaddingStyle.stretchWidth = false;
            helpBoxNoPaddingStyle.stretchHeight = false;
            //_styleHelpBoxNoPad.normal.background = Texture2D.whiteTexture;
        }

        internal static void Show(AnimationCollapseGroup section, int indentLevel = 0)
        {
            if (section.showOnlyInEditMode && Application.isPlaying) return;

            float headerGlow = Mathf.Lerp(0.5f, 0.85f, section.faded);
            //float headerGlow = Mathf.Lerp(0.85f, 1f, section.Faded);
            if (EditorGUIUtility.isProSkin)
            {
                GUI.backgroundColor = section.backgroundColor * new Color(headerGlow, headerGlow, headerGlow, 1f);
            }
            else
            {
                headerGlow = Mathf.Lerp(0.75f, 1f, section.faded);
                GUI.backgroundColor = section.backgroundColor * new Color(headerGlow, headerGlow, headerGlow, 1f);
            }

            GUILayout.BeginVertical(helpBoxNoPaddingStyle);
            GUILayout.Box(GUIContent.none, EditorStyles.miniButton, GUILayout.ExpandWidth(true));
            GUI.backgroundColor = Color.white;
            Rect buttonRect = GUILayoutUtility.GetLastRect();
            if (Event.current.type != EventType.Layout)
            {
                buttonRect.xMin += indentLevel * EditorGUIUtility.fieldWidth / 3f;
                EditorGUI.indentLevel++;
                EditorGUIUtility.SetIconSize(new Vector2(16f, 16f));
                section.isExpanded = EditorGUI.Foldout(buttonRect, section.isExpanded, section.Label, true,
                    buttonFoldoutStyle);
                EditorGUIUtility.SetIconSize(Vector2.zero);
                EditorGUI.indentLevel--;
            }

            if (EditorGUILayout.BeginFadeGroup(section.faded))
            {
                section.Invoke();
            }

            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }

        internal static void Show(string label, ref bool isExpanded, System.Action action, bool showOnlyInEditMode)
        {
            if (showOnlyInEditMode && Application.isPlaying) return;
            if (BeginShow(label, ref isExpanded, Color.white))
            {
                action.Invoke();
            }

            EndShow();
        }

        internal static bool BeginShow(string label, ref bool isExpanded, Color tintColor)
        {
            GUI.color = Color.white;
            GUI.backgroundColor = Color.clear;
            if (isExpanded)
            {
                GUI.color = Color.white;
                GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 0.1f);
                if (EditorGUIUtility.isProSkin)
                {
                    GUI.backgroundColor = Color.black;
                }
            }

            GUILayout.BeginVertical(collapsableSectionStyle);
            GUI.color = tintColor;
            GUI.backgroundColor = Color.white;
            if (GUILayout.Button(label, EditorStyles.toolbarButton))
            {
                isExpanded = !isExpanded;
            }

            GUI.color = Color.white;

            return isExpanded;
        }

        internal static void EndShow()
        {
            GUILayout.EndVertical();
        }
    }
}