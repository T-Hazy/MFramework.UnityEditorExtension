using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;

namespace MFramework.EditorExtensions
{
    public class InspectorEditor : UnityEditor.Editor
    {
        #region 基础编辑器方法

        private SerializedProperty scriptProperty;

        protected int IndentLevel
        {
            get => EditorGUI.indentLevel;
            set => EditorGUI.indentLevel += value;
        }

        protected static Texture2D GetIcon(string iconName) => Resources.Load<Texture2D>(iconName);

        public void DrawIcon(string iconName)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            var icon = GetIcon(iconName);
            if (icon != null)
            {
                GUILayout.Label(new GUIContent(icon));
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }


        /// <summary>
        /// 在检查器上绘制一个属性字段
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <param name="options">GUI布局选项</param>
        protected void PropertyField(string propertyName, params GUILayoutOption[] options) =>
            EditorGUILayout.PropertyField(serializedObject.FindProperty(propertyName), options);

        /// <summary>
        /// 在检查器上绘制一个属性字段
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <param name="includeChildren">包括子字段</param>
        /// <param name="options">GUI布局选项</param>
        protected void PropertyField(string propertyName, bool includeChildren, params GUILayoutOption[] options) =>
            EditorGUILayout.PropertyField(serializedObject.FindProperty(propertyName), includeChildren, options);


        /// <summary>
        /// 在检查器上绘制一个带有工具提示的属性字段
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <param name="toolTip">提示内容</param>
        /// <param name="options">GUI布局选项</param>
        protected void PropertyField(string propertyName, string toolTip, params GUILayoutOption[] options) =>
            EditorGUILayout.PropertyField(serializedObject.FindProperty(propertyName),
                new GUIContent(propertyName, toolTip), options);

        /// <summary>
        /// 在检查器上绘制一个带有工具提示的属性字段
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <param name="toolTip">提示内容</param>
        /// <param name="includeChildren">包括子字段</param>
        /// <param name="options">GUI布局选项</param>
        protected void PropertyField(string propertyName, string toolTip, bool includeChildren,
            params GUILayoutOption[] options) =>
            EditorGUILayout.PropertyField(serializedObject.FindProperty(propertyName),
                new GUIContent(propertyName, toolTip), includeChildren, options);


        /// <summary>
        /// 在检查器上绘制一个只读属性字段
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <param name="options">GUI布局选项</param>
        protected void ReadOnlyProperty(string propertyName, params GUILayoutOption[] options)
        {
            GUI.enabled = false;
            PropertyField(propertyName, options);
            GUI.enabled = true;
        }

        /// <summary>
        /// 在检查器上绘制一个只读属性字段
        /// </summary>
        /// <param name="propertyName">Property Name</param>
        /// <param name="includeChildren">包括子字段</param>
        /// <param name="options">GUI布局选项</param>
        protected void ReadOnlyProperty(string propertyName, bool includeChildren, params GUILayoutOption[] options)
        {
            GUI.enabled = false;
            PropertyField(propertyName, includeChildren, options);
            GUI.enabled = true;
        }

        /// <summary>
        /// 在检查器上绘制一个带有工具提示的只读属性字段
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <param name="tooltip">提示内容</param>
        /// <param name="options">GUI布局选项</param>
        protected void ReadOnlyProperty(string propertyName, string tooltip, params GUILayoutOption[] options)
        {
            GUI.enabled = false;
            PropertyField(propertyName, tooltip, options);
            GUI.enabled = true;
        }

        /// <summary>
        /// 在检查器上绘制一个只读属性字段的集合
        /// </summary>
        /// <param name="propertyNames">属性名集合</param>
        /// <param name="options">GUI布局选项</param>
        protected void ReadOnlyProperties(IEnumerable<string> propertyNames, params GUILayoutOption[] options)
        {
            GUI.enabled = false;
            foreach (var propertyName in propertyNames)
            {
                PropertyField(propertyName, options);
            }

            GUI.enabled = true;
        }

        /// <summary>
        /// 在检查器上绘制一个只读属性字段的集合
        /// </summary>
        /// <param name="propertyNames">属性名集合</param>
        /// <param name="includeChildren">包括子字段</param>
        /// <param name="options">GUI布局选项</param>
        protected void ReadOnlyProperties(IEnumerable<string> propertyNames, bool includeChildren,
            params GUILayoutOption[] options)
        {
            GUI.enabled = false;
            foreach (var property in propertyNames)
            {
                PropertyField(property, includeChildren, options);
            }

            GUI.enabled = true;
        }

        /// <summary>
        /// 在检查器上绘制缩进属性字段
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <param name="indentLevel">缩进级别</param>
        /// <param name="options">GUI布局选项</param>
        protected void IndentPropertyField(string propertyName, int indentLevel = 1, params GUILayoutOption[] options)
        {
            EditorGUI.indentLevel += indentLevel;
            PropertyField(propertyName, options);
            EditorGUI.indentLevel -= indentLevel;
        }

        /// <summary>
        /// 在检查器上绘制一个带工具提示的缩进属性字段
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <param name="tooltip">提示内容</param>
        /// <param name="indentLevel">缩进级别</param>
        /// <param name="options">GUI布局选项</param>
        protected void IndentPropertyField(string propertyName, string tooltip, int indentLevel = 1,
            params GUILayoutOption[] options)
        {
            EditorGUI.indentLevel += indentLevel;
            PropertyField(propertyName, tooltip, options);
            EditorGUI.indentLevel -= indentLevel;
        }

        /// <summary>
        /// 在检查器上绘制一些缩进属性字段
        /// </summary>
        /// <param name="propertyNames">属性名集合</param>
        /// <param name="indentLevel">缩进级别</param>
        /// <param name="options">GUI布局选项</param>
        protected void IndentProperties(IEnumerable<string> propertyNames, int indentLevel = 1,
            params GUILayoutOption[] options)
        {
            EditorGUI.indentLevel += indentLevel;
            foreach (var propertyName in propertyNames)
            {
                PropertyField(propertyName, options);
            }

            EditorGUI.indentLevel -= indentLevel;
        }

        /// <summary>
        /// 在检查器上绘制一些缩进的只读属性字段
        /// </summary>
        /// <param name="propertyNames">Property Names</param>
        /// <param name="indentLevel">缩进级别</param>
        /// <param name="options">GUI布局选项</param>
        protected void IndentReadOnlyProperties(IEnumerable<string> propertyNames, int indentLevel = 1,
            params GUILayoutOption[] options)
        {
            EditorGUI.indentLevel += indentLevel;
            foreach (var propertyName in propertyNames)
            {
                ReadOnlyProperty(propertyName, options);
            }

            EditorGUI.indentLevel -= indentLevel;
        }

        /// <summary>
        /// 在检查器上绘制一个缩进的只读属性字段
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <param name="indentLevel">缩进级别</param>
        /// <param name="options">GUI布局选项</param>
        protected void IndentReadOnlyPropertyField(string propertyName, int indentLevel = 1,
            params GUILayoutOption[] options)
        {
            EditorGUI.indentLevel += indentLevel;
            ReadOnlyProperty(propertyName, options);
            EditorGUI.indentLevel -= indentLevel;
        }

        /// <summary>
        /// 在检查器上绘制一个带工具提示的缩进只读属性字段
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <param name="tooltip">提示内容</param>
        /// <param name="indentLevel">缩进级别</param>
        /// <param name="options">GUI布局选项</param>
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        protected void IndentReadOnlyPropertyField(string propertyName, string tooltip, int indentLevel = 1,
            params GUILayoutOption[] options)
        {
            EditorGUI.indentLevel += indentLevel;
            ReadOnlyProperty(propertyName, tooltip, options);
            EditorGUI.indentLevel -= indentLevel;
        }

        /// <summary>
        /// 在检查器上绘制脚本属性字段
        /// </summary>
        protected void ScriptProperty()
        {
            ReadOnlyProperty("m_Script");
        }

        protected void Space(int width) => EditorGUILayout.Space(width);
        protected void Space(int width, bool expand) => EditorGUILayout.Space(width, expand);

        protected void HelpBox(string message, MessageType messageType) =>
            EditorGUILayout.HelpBox(message, messageType);

        protected void Header(string label) => EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

        /// <summary>
        /// 绘制水平线
        /// </summary>
        /// <param name="color">线颜色</param>
        /// <param name="lineHeight">线粗细</param>
        protected void DrawHorizontalLine(Color color, float lineHeight = 1.25f)
        {
            EditorGUILayout.Space(0.5f);
            var lineRect = EditorGUILayout.GetControlRect(false, lineHeight);
            EditorGUI.DrawRect(lineRect, color);
            EditorGUILayout.Space(0.5f);
        }

        /// <summary>
        /// 绘制缩进文本
        /// </summary>
        /// <param name="label">文本内容</param>
        /// <param name="indentLevel">缩进级别</param>
        /// <param name="options">GUI布局选项</param>
        protected void IndentLabel(string label, int indentLevel = 1, params GUILayoutOption[] options)
        {
            EditorGUI.indentLevel += indentLevel;
            EditorGUILayout.LabelField(label, options);
            EditorGUI.indentLevel -= indentLevel;
        }

        /// <summary>
        /// 绘制缩进文本
        /// </summary>
        /// <param name="label">文本内容</param>
        /// <param name="style">文本GUI样式</param>
        /// <param name="indentLevel">缩进级别</param>
        /// <param name="options">GUI布局选项</param>
        protected void IndentLabel(string label, GUIStyle style, int indentLevel = 1,
            params GUILayoutOption[] options)
        {
            EditorGUI.indentLevel += indentLevel;
            EditorGUILayout.LabelField(label, style, options);
            EditorGUI.indentLevel -= indentLevel;
        }

        /// <summary>
        /// 绘制缩进文本
        /// </summary>
        /// <param name="label">文本内容</param>
        /// <param name="indentLevel">缩进级别</param>
        /// <param name="options">GUI布局选项</param>
        protected void IndentLabel(GUIContent label, int indentLevel = 1, params GUILayoutOption[] options)
        {
            EditorGUI.indentLevel += indentLevel;
            EditorGUILayout.LabelField(label, options);
            EditorGUI.indentLevel -= indentLevel;
        }


        /// <summary>
        /// 绘制缩进文本
        /// </summary>
        /// <param name="label">文本1内容</param>
        /// <param name="label2">文本2内容</param>
        /// <param name="indentLevel">缩进级别</param>
        /// <param name="options">GUI布局选项</param>
        protected void IndentLabel(GUIContent label, GUIContent label2, int indentLevel = 1,
            params GUILayoutOption[] options)
        {
            EditorGUI.indentLevel += indentLevel;
            EditorGUILayout.LabelField(label, label2, options);
            EditorGUI.indentLevel -= indentLevel;
        }


        /// <summary>
        /// 绘制缩进文本
        /// </summary>
        /// <param name="label">文本1内容</param>
        /// <param name="label2">文本2内容</param>
        /// <param name="style">文本样式</param>
        /// <param name="indentLevel">缩进级别</param>
        /// <param name="options">GUI布局选项</param>
        protected void IndentLabel(GUIContent label, GUIContent label2, GUIStyle style,
            int indentLevel = 1, params GUILayoutOption[] options)
        {
            EditorGUI.indentLevel += indentLevel;
            EditorGUILayout.LabelField(label, label2, style, options);
            EditorGUI.indentLevel -= indentLevel;
        }

        private void FixEditorBug()
        {
            var remainingBuggedEditors = FindObjectsOfType<InspectorEditor>();
            foreach (var editor in remainingBuggedEditors)
            {
                if (editor == this) continue;
                DestroyImmediate(editor);
            }
        }

        /// <summary>
        /// 应用属性修改
        /// </summary>
        protected void ApplyModifiedProperties()
        {
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }

        #endregion

        #region 动画折叠组

        private static readonly Dictionary<string, AnimationCollapseGroup> groups =
            new Dictionary<string, AnimationCollapseGroup>();

        /// <summary>
        /// 绘制动画折叠组
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="drawMethod">绘制方法</param>
        /// <param name="color">颜色</param>
        /// <param name="editor">编辑器对象</param>
        /// <param name="indent">缩进</param>
        /// <param name="showOnlyInEditorMode">只在编辑器模式下显示</param>
        /// <param name="isDefaultExpanded">默认展开</param>
        protected void DrawAnimationCollapseGroup(string title, Action drawMethod, Color color,
            // ReSharper disable once RedundantNameQualifier
            UnityEditor.Editor editor, int indent = 0, bool showOnlyInEditorMode = false,
            bool isDefaultExpanded = false)
        {
            if (groups.TryGetValue($"{editor.GetInstanceID()}.{title}", out var group))
            {
                AnimationCollapseGroup.Show(group, indent);
            }
            else
            {
                var newGroup = new AnimationCollapseGroup(title, showOnlyInEditorMode, isDefaultExpanded, drawMethod,
                    editor, color);
                AnimationCollapseGroup.Show(newGroup, indent);
                groups.Add($"{editor.GetInstanceID()}.{title}", newGroup);
            }
        }

        #endregion

        protected virtual void OnEnable()
        {
            FixEditorBug();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            //动画折叠组的的样式
            AnimationCollapseGroup.CreateStyles();
            ScriptProperty();
        }

        protected virtual void OnDisable()
        {
            foreach (var collapseSection in groups.Values)
            {
                collapseSection.Save();
            }
        }
    }
}