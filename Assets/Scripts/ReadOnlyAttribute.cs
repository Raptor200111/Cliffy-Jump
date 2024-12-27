using UnityEngine;
using UnityEditor;

public class ReadOnlyAttribute : PropertyAttribute { }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false; // Deshabilita la edici�n en el Inspector
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;  // Habilita la edici�n para otros elementos
    }
}
#endif
