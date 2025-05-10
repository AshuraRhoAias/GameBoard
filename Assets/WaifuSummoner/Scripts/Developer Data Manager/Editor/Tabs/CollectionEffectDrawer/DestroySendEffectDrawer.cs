// Assets/WaifuSummoner/Scripts/Developer Data Manager/Editor/Drawers/DestroySendEffectDrawer.cs
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[EffectDrawer(EffectType.DestroySend, "destroySendEffect")]
public class DestroySendEffectDrawer : IEffectDrawer
{
    public void Draw(SerializedProperty prop)
    {
        if (prop == null) return;

        // 1) Target
        var targetProp = prop.FindPropertyRelative("targetType");
        EditorGUILayout.PropertyField(targetProp, new GUIContent("Target"));
        var target = (Target)targetProp.enumValueIndex;
        if (target == Target.None) return;

        // 2) Card Type
        var cardTypeProp = prop.FindPropertyRelative("cardType");
        EditorGUILayout.PropertyField(cardTypeProp, new GUIContent("Card Type"));
        var cardType = (CardType)cardTypeProp.enumValueIndex;

        // 3) Action: Destroy vs Send
        var actionProp = prop.FindPropertyRelative("actionType");
        EditorGUILayout.PropertyField(actionProp, new GUIContent("Action"));
        var action = (DestroySendAction)actionProp.enumValueIndex;

        // 3.1) Si es Enchantment + Destroy, mostrar filtro de posición
        if (cardType == CardType.Enchantment && action == DestroySendAction.Destroy)
        {
            var posProp = prop.FindPropertyRelative("enchantmentPosition");
            if (posProp != null)
                EditorGUILayout.PropertyField(posProp, new GUIContent("Enchantment Position"));
        }

        // 4) If Send, choose Location
        if (action == DestroySendAction.Send)
        {
            var locProp = prop.FindPropertyRelative("location");
            if (locProp != null)
                EditorGUILayout.PropertyField(locProp, new GUIContent("Location"));
        }

        // 5) Duration — para “regresar” la carta (solo si tienes duration en tu data)
        var durProp = prop.FindPropertyRelative("duration");
        if (durProp != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(durProp, new GUIContent("Duration"));
            var dur = (Duration)durProp.enumValueIndex;

            // 5.1) Hasta la siguiente etapa
            if (dur == Duration.UntilTheNext)
            {
                var stageProp = prop.FindPropertyRelative("untilStage");
                if (stageProp != null)
                    EditorGUILayout.PropertyField(stageProp, new GUIContent("Until Stage"));

                var turnsProp = prop.FindPropertyRelative("durationTurns");
                if (turnsProp != null)
                {
                    turnsProp.intValue = Mathf.Max(1, turnsProp.intValue);
                    EditorGUILayout.PropertyField(turnsProp, new GUIContent("Turns"));
                }
            }
            // 5.2) Número de turnos fijo
            else if (dur == Duration.ForNumberTurns || dur == Duration.ForNumberOfYourTurns)
            {
                var turnsProp = prop.FindPropertyRelative("durationTurns");
                if (turnsProp != null)
                {
                    turnsProp.intValue = Mathf.Max(1, turnsProp.intValue);
                    EditorGUILayout.PropertyField(turnsProp, new GUIContent("Turns"));
                }
            }
        }
    }
}
#endif
