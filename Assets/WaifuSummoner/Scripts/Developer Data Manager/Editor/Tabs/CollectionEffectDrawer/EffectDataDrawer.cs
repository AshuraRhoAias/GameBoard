using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EffectData))]
public class EffectDataDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        SerializedProperty effectName = property.FindPropertyRelative("effectName");
        SerializedProperty trigger = property.FindPropertyRelative("trigger");
        SerializedProperty effectType = property.FindPropertyRelative("effectType");
        SerializedProperty defeatEffect = property.FindPropertyRelative("defeatEffect");
        SerializedProperty sendWaifuEffect = property.FindPropertyRelative("sendWaifuEffect");
        SerializedProperty description = property.FindPropertyRelative("effectDescription");

        float lineHeight = EditorGUIUtility.singleLineHeight + 2;
        Rect currentRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        // Nombre del efecto
        EditorGUI.PropertyField(currentRect, effectName);
        currentRect.y += lineHeight;

        // Trigger
        EditorGUI.PropertyField(currentRect, trigger);
        currentRect.y += lineHeight;

        // EffectType
        EditorGUI.PropertyField(currentRect, effectType);
        currentRect.y += lineHeight;

        // Mostrar el bloque dependiendo del EffectType
        switch ((EffectType)effectType.enumValueIndex)
        {
            case EffectType.Defeat:
                EditorGUI.PropertyField(
                    new Rect(currentRect.x, currentRect.y, currentRect.width, EditorGUI.GetPropertyHeight(defeatEffect, true)),
                    defeatEffect, true);
                currentRect.y += EditorGUI.GetPropertyHeight(defeatEffect, true) + 4;
                break;

            case EffectType.SendWaifu:
                EditorGUI.PropertyField(
                    new Rect(currentRect.x, currentRect.y, currentRect.width, EditorGUI.GetPropertyHeight(sendWaifuEffect, true)),
                    sendWaifuEffect, true);
                currentRect.y += EditorGUI.GetPropertyHeight(sendWaifuEffect, true) + 4;
                break;

            default:
                EditorGUI.HelpBox(currentRect, "No data fields for this effect type.", MessageType.Info);
                currentRect.y += lineHeight + 10;
                break;
        }

        // Descripción del efecto (siempre visible)
        EditorGUI.PropertyField(currentRect, description);
        currentRect.y += EditorGUI.GetPropertyHeight(description) + 4;

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float totalHeight = 0;
        float lineHeight = EditorGUIUtility.singleLineHeight + 2;

        SerializedProperty effectType = property.FindPropertyRelative("effectType");
        SerializedProperty defeatEffect = property.FindPropertyRelative("defeatEffect");
        SerializedProperty sendWaifuEffect = property.FindPropertyRelative("sendWaifuEffect");
        SerializedProperty description = property.FindPropertyRelative("effectDescription");

        totalHeight += lineHeight * 3; // effectName, trigger, effectType

        // Suma la altura dependiendo del tipo de efecto
        switch ((EffectType)effectType.enumValueIndex)
        {
            case EffectType.Defeat:
                totalHeight += EditorGUI.GetPropertyHeight(defeatEffect, true) + 4;
                break;
            case EffectType.SendWaifu:
                totalHeight += EditorGUI.GetPropertyHeight(sendWaifuEffect, true) + 4;
                break;
            default:
                totalHeight += lineHeight + 10;
                break;
        }

        // Altura de la descripción
        totalHeight += EditorGUI.GetPropertyHeight(description) + 4;

        return totalHeight;
    }
}
