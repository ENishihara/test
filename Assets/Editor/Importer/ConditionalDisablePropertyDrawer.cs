using UnityEngine;
using UnityEditor;

/// <summary>
/// ConditionalDisableAttribute用PropertyDrawer
/// </summary>
[CustomPropertyDrawer(typeof(ConditionalDisableAttribute))]
public class ConditionalDisablePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalDisableAttribute condAttr = (ConditionalDisableAttribute)attribute;
        bool enable = GetConditionalDisableAttributeResult(condAttr, property);

        bool enabled = GUI.enabled;
        GUI.enabled = enable;
        if (!condAttr.disable || enable)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }

        GUI.enabled = enabled;
    }

    /// <summary>
    /// 入力可否を返す
    /// </summary>
    bool GetConditionalDisableAttributeResult(ConditionalDisableAttribute condAttr,
                                              SerializedProperty property)
    {
        bool enabled = true;
        string propertyPath = property.propertyPath;
        string conditionPath = propertyPath.Replace(property.name, condAttr.sourceField);
        SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);
        if (sourcePropertyValue == null)
        {
            return enabled;
        }
        if (sourcePropertyValue.propertyType == SerializedPropertyType.Enum)
        {
            enabled = condAttr.enableValue == sourcePropertyValue.enumValueIndex;
        }
        else if (sourcePropertyValue.propertyType == SerializedPropertyType.Integer)
        {
            enabled = condAttr.enableValue == sourcePropertyValue.intValue;
        }

        return enabled;
    }
}