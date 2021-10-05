using UnityEngine;
using UnityEditor;
using PHC.Assets.Scripts.Art;

namespace PHC.Art.Editor
{
    [CustomPropertyDrawer(typeof(GColor))]
    [CanEditMultipleObjects]
    public class GColorEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty propM_Value = property.FindPropertyRelative("m_value");
            GColor color = propM_Value.intValue;
            color = EditorGUI.ColorField(position, label, color);
            propM_Value.intValue = color.m_value;
        }
    }
}
