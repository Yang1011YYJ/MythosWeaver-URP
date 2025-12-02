using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SmallHeaderAttribute : PropertyAttribute
{
    public string title;
    public SmallHeaderAttribute(string title)
    {
        this.title = title;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SmallHeaderAttribute))]
public class SmallHeaderDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SmallHeaderAttribute header = (SmallHeaderAttribute)attribute;

        GUIStyle style = new GUIStyle(EditorStyles.label);
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = new Color(0.7f, 0.7f, 0.7f);

        //EditorGUILayout.Space(2);
        EditorGUILayout.LabelField(header.title, style);
        EditorGUILayout.PropertyField(property, label, true);
    }
}
#endif
