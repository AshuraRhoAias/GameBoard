#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Drawer para HealBurnEffectData.
/// Registrado para EffectType.HealBurn y la propiedad "healBurnEffect".
/// </summary>
[EffectDrawer(EffectType.HealBurn, "healBurnEffect")]
public class HealBurnEffectDrawer : IEffectDrawer
{
    /// <summary>
    /// prop ya apunta a SerializedProperty("healBurnEffect") dentro de EffectData.
    /// </summary>
    public void Draw(SerializedProperty prop)
    {
        if (prop == null)
            return;

        // 1️⃣ Acción (None / Increase / Decrease)
        var actionProp = prop.FindPropertyRelative("action");
        EditorGUILayout.PropertyField(actionProp, new GUIContent("Action"));

        var action = (IncreaseDecrease)actionProp.enumValueIndex;
        if (action != IncreaseDecrease.None)
        {
            // 2️⃣ Target Side (Both / Enemy / User)
            var sideProp = prop.FindPropertyRelative("targetSide");
            EditorGUILayout.PropertyField(sideProp, new GUIContent("Target Side"));

            // 3️⃣ Amount to heal or burn
            var amountProp = prop.FindPropertyRelative("amount");
            EditorGUILayout.PropertyField(amountProp, new GUIContent("Amount"));
        }
    }
}
#endif
