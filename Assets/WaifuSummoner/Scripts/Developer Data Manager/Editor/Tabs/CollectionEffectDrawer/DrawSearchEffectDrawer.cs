#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

[EffectDrawer(EffectType.DrawSearch, "drawSearchEffect")]
public class DrawSearchEffectDrawer : IEffectDrawer
{
    public void Draw(SerializedProperty prop)
    {
        if (prop == null) return;

        // ─────────────────────────────────────────────────────
        // 1) Mode (Draw / Search)
        //    Si es None, detenemos aquí.
        // ─────────────────────────────────────────────────────
        var modeProp = prop.FindPropertyRelative("mode");
        if (modeProp == null) return;
        EditorGUILayout.PropertyField(modeProp, new GUIContent("Mode"));
        var mode = (DrawSearch)modeProp.enumValueIndex;
        if (mode == DrawSearch.None)
            return;

        // ─────────────────────────────────────────────────────
        // 2a) Si es Search → mostrar searchSource
        // ─────────────────────────────────────────────────────
        if (mode == DrawSearch.Search)
        {
            var srcProp = prop.FindPropertyRelative("searchSource");
            if (srcProp != null)
                EditorGUILayout.PropertyField(srcProp, new GUIContent("Search From"));
        }

        // ─────────────────────────────────────────────────────
        // 3) Amount (siempre ≥1, tanto para Draw como para Search)
        // ─────────────────────────────────────────────────────
        var amtProp = prop.FindPropertyRelative("amount");
        if (amtProp != null)
        {
            amtProp.intValue = Mathf.Max(1, amtProp.intValue);
            EditorGUILayout.PropertyField(amtProp, new GUIContent("Amount"));
        }

        // ─────────────────────────────────────────────────────
        // 4) Destination
        //    A dónde enviar las cartas (Draw o Search)
        // ─────────────────────────────────────────────────────
        var destProp = prop.FindPropertyRelative("destination");
        if (destProp != null)
            EditorGUILayout.PropertyField(destProp, new GUIContent("Destination"));
    }
}
#endif

