// Assets/WaifuSummoner/Scripts/Developer Data Manager/Editor/Tabs/CollectionEffectDrawers/SendHandEffectDrawer.cs
#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[EffectDrawer(EffectType.SendHand, "sendHandEffect")]
public class SendHandEffectDrawer : IEffectDrawer
{
    public void Draw(SerializedProperty shProp)
    {
        if (shProp == null) return;

        // 1) Target (None/All/Select/Random/Situational)
        var selProp = shProp.FindPropertyRelative("selectionType");
        if (selProp == null) return;
        EditorGUILayout.PropertyField(selProp, new GUIContent("Target"));
        var sel = (Target)selProp.enumValueIndex;
        if (sel == Target.None)
            return;

        // 2) Target Side (siempre que no sea None)
        var sideProp = shProp.FindPropertyRelative("targetSide");
        if (sideProp != null)
            EditorGUILayout.PropertyField(sideProp, new GUIContent("Target Side"));

        // 3) Amount (solo para Select/Random/Situational)
        if (sel == Target.Select || sel == Target.Random || sel == Target.Situational)
        {
            var amtProp = shProp.FindPropertyRelative("amount");
            if (amtProp != null)
            {
                amtProp.intValue = Mathf.Max(1, amtProp.intValue);
                EditorGUILayout.PropertyField(amtProp, new GUIContent("Amount"));
            }
        }

        // 4) Location
        var locProp = shProp.FindPropertyRelative("location");
        if (locProp != null)
            EditorGUILayout.PropertyField(locProp, new GUIContent("Location"));

        // 5) Filters (dinámico por CardType)
        var listProp = shProp.FindPropertyRelative("filterCardTypes");
        if (listProp == null || !listProp.isArray)
            return;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Filters", EditorStyles.boldLabel);

        // Mostrar cada filtro con botón de eliminar
        int removeIdx = -1;
        for (int i = 0; i < listProp.arraySize; i++)
        {
            var elem = listProp.GetArrayElementAtIndex(i);
            if (elem == null) continue;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(elem, GUIContent.none);
            if (GUILayout.Button("✕", GUILayout.Width(20)))
                removeIdx = i;
            EditorGUILayout.EndHorizontal();
        }
        if (removeIdx >= 0)
            listProp.DeleteArrayElementAtIndex(removeIdx);

        // Añadir nuevo filtro con el primer CardType disponible
        var allTypes = Enum.GetValues(typeof(CardType)).Cast<CardType>();
        var usedTypes = Enumerable.Range(0, listProp.arraySize)
            .Select(i => (CardType)listProp.GetArrayElementAtIndex(i).enumValueIndex)
            .ToList();
        var available = allTypes.Except(usedTypes).ToList();

        if (available.Count > 0 && GUILayout.Button("+ Add Filter"))
        {
            listProp.InsertArrayElementAtIndex(listProp.arraySize);
            var newElem = listProp.GetArrayElementAtIndex(listProp.arraySize - 1);
            newElem.enumValueIndex = (int)available[0];
        }
    }
}
#endif
