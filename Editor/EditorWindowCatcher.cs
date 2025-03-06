using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MFramework.EditorExtensions
{
    public class EditorWindowCatcher : EditorWindow
    {
        private static List<Type> windowsList = new List<Type>();

        public static void ShowInEditor()
        {
            var window = GetWindow(typeof(EditorWindowCatcher));
            window.titleContent = new GUIContent("EditorWindowCatcher");
            window.minSize = new Vector2(400, 800);
            window.position = new Rect(900, 300, 800, 800);
            window.Show();
            windowsList = CaptureAllWindows();
        }

        private static List<Type> CaptureAllWindows()
        {
            Assembly assembly = typeof(EditorWindow).Assembly;
            Type[] types = assembly.GetTypes();
            List<Type> windowList = new List<Type>();
            foreach (var type in types)
            {
                if (type.BaseType == typeof(EditorWindow))
                {
                    windowList.Add(type);
                }
            }

            windowList.Sort((a, b) => string.Compare(a.Name, b.Name));
            return windowList;
        }

        private Vector2 pos = Vector2.zero;
        private readonly TextEditor textEditor = new TextEditor();
        private bool showTypeFullName = false;

        private void OnGUI()
        {
            pos = GUILayout.BeginScrollView(pos);
            if (showTypeFullName)
            {
                if (GUILayout.Button("ShowSimpleName", GUILayout.Height(25)))
                {
                    showTypeFullName = false;
                }
            }
            else
            {
                if (GUILayout.Button("ShowFullName", GUILayout.Height(25)))
                {
                    showTypeFullName = true;
                }
            }


            foreach (var type in windowsList)
            {
                GUILayout.BeginHorizontal();
                GUILayout.TextField(showTypeFullName ? type.FullName : type.Name, 200, GUILayout.Height(25));
                if (GUILayout.Button("CopyOpenCode", GUILayout.Width(120), GUILayout.Height(25)))
                {
                    textEditor.text = $"EditorWindow.GetWindow(\"{type}\".ToType()).Show();";
                    textEditor.SelectAll();
                    textEditor.Copy();
                    Debug.Log("The opening code is copied to the clipboard");
                }

                if (GUILayout.Button("Open", GUILayout.Width(80), GUILayout.Height(25)))
                {
                    EditorWindow.GetWindow(type).Show();
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }
    }
}