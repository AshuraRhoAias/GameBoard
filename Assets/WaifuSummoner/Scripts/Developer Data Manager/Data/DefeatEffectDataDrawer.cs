using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(DefeatEffectData))]
public class DefeatEffectDataDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.indentLevel++;

        SerializedProperty targetType = property.FindPropertyRelative("targetType");
        SerializedProperty targetSide = property.FindPropertyRelative("targetSide");
        SerializedProperty amount = property.FindPropertyRelative("amount");
        SerializedProperty filterType = property.FindPropertyRelative("filterType");
        SerializedProperty summonConditionFilter = property.FindPropertyRelative("summonConditionFilter");
        SerializedProperty roleFilter = property.FindPropertyRelative("roleFilter");
        SerializedProperty elementFilter = property.FindPropertyRelative("elementFilter");
        SerializedProperty situationalHighLow = property.FindPropertyRelative("situationalHighLow");
        SerializedProperty situationalStat = property.FindPropertyRelative("situationalStat");
        SerializedProperty situationalTieBreaker = property.FindPropertyRelative("situationalTieBreaker");

        Rect line = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(line, targetType);

        line.y += EditorGUIUtility.singleLineHeight + 2;

        EditorGUI.PropertyField(line, targetSide);
        line.y += EditorGUIUtility.singleLineHeight + 2;

        if ((Target)targetType.enumValueIndex == Target.Select ||
            (Target)targetType.enumValueIndex == Target.Random)
        {
            EditorGUI.PropertyField(line, amount);
            line.y += EditorGUIUtility.singleLineHeight + 2;
        }

        if ((Target)targetType.enumValueIndex == Target.All ||
            (Target)targetType.enumValueIndex == Target.Select ||
            (Target)targetType.enumValueIndex == Target.Random)
        {
            EditorGUI.PropertyField(line, filterType);
            line.y += EditorGUIUtility.singleLineHeight + 2;

            var filter = (DefeatFilterType)filterType.enumValueIndex;
            if (filter == DefeatFilterType.SummonCondition)
            {
                EditorGUI.PropertyField(line, summonConditionFilter);
                line.y += EditorGUIUtility.singleLineHeight + 2;
            }
            else if (filter == DefeatFilterType.Role)
            {
                EditorGUI.PropertyField(line, roleFilter);
                line.y += EditorGUIUtility.singleLineHeight + 2;
            }
            else if (filter == DefeatFilterType.Element) // Aquí es ELEMENT, no Attribute
            {
                EditorGUI.PropertyField(line, elementFilter);
                line.y += EditorGUIUtility.singleLineHeight + 2;
            }
        }

        if ((Target)targetType.enumValueIndex == Target.Situational)
        {
            EditorGUI.PropertyField(line, situationalHighLow);
            line.y += EditorGUIUtility.singleLineHeight + 2;

            EditorGUI.PropertyField(line, situationalStat);
            line.y += EditorGUIUtility.singleLineHeight + 2;

            EditorGUI.PropertyField(line, situationalTieBreaker);
            line.y += EditorGUIUtility.singleLineHeight + 2;

            EditorGUI.PropertyField(line, filterType);
            line.y += EditorGUIUtility.singleLineHeight + 2;

            var filter = (DefeatFilterType)filterType.enumValueIndex;
            if (filter == DefeatFilterType.SummonCondition)
            {
                EditorGUI.PropertyField(line, summonConditionFilter);
                line.y += EditorGUIUtility.singleLineHeight + 2;
            }
            else if (filter == DefeatFilterType.Role)
            {
                EditorGUI.PropertyField(line, roleFilter);
                line.y += EditorGUIUtility.singleLineHeight + 2;
            }
            else if (filter == DefeatFilterType.Element) // También aquí es ELEMENT
            {
                EditorGUI.PropertyField(line, elementFilter);
                line.y += EditorGUIUtility.singleLineHeight + 2;
            }
        }

        EditorGUI.indentLevel--;
        EditorGUI.EndProperty();
    }
}
