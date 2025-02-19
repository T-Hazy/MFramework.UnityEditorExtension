using UnityEditor;
using UnityEngine;

namespace MFramework.EditorExtension
{
    public static class RectExpansion
    {
        public static Rect DrawOutline(this Rect rect, Color color, int thickness) {
            // Top
            EditorGUI.DrawRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
            // Bottom
            EditorGUI.DrawRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
            // Left
            EditorGUI.DrawRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
            // Right
            EditorGUI.DrawRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
            return rect;
        }
    }
}