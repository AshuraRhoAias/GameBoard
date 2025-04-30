#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[EffectDrawer(EffectType.EnchantSummon, "enchantSummonEffect")]
public class EnchantSummonEffectDrawer : IEffectDrawer
{
    public void Draw(SerializedProperty prop)
    {
        if (prop == null) return;

        // 1) Location
        //    Siempre visible.
        var locProp = prop.FindPropertyRelative("location");
        if (locProp != null)
            EditorGUILayout.PropertyField(locProp, new GUIContent("Location"));

        // 2) Target
        //    Si es None, detenemos aquí.
        var targetProp = prop.FindPropertyRelative("target");
        if (targetProp == null) return;
        EditorGUILayout.PropertyField(targetProp, new GUIContent("Target"));
        var target = (Target)targetProp.enumValueIndex;
        if (target == Target.None)
            return;

        // 3) Amount
        //    Solo para Select/Random/Situational.
        if (target == Target.Select
         || target == Target.Random
         || target == Target.Situational)
        {
            var amtProp = prop.FindPropertyRelative("amount");
            if (amtProp != null)
            {
                amtProp.intValue = Mathf.Max(1, amtProp.intValue);
                EditorGUILayout.PropertyField(amtProp, new GUIContent("Amount"));
            }
        }

        // 4) Situational extras
        //    Solo si target == Situational.
        if (target == Target.Situational)
        {
            var hlProp = prop.FindPropertyRelative("highLow");
            var stProp = prop.FindPropertyRelative("statType");
            var tbProp = prop.FindPropertyRelative("tieBreaker");

            if (hlProp != null) EditorGUILayout.PropertyField(hlProp, new GUIContent("Highest / Lowest"));
            if (stProp != null) EditorGUILayout.PropertyField(stProp, new GUIContent("Stat to Compare"));
            if (tbProp != null) EditorGUILayout.PropertyField(tbProp, new GUIContent("Tie Breaker"));
        }

        // 5) Target Side
        //    Siempre visible después de elegir un target distinto de None.
        var sideProp = prop.FindPropertyRelative("targetSide");
        if (sideProp != null)
            EditorGUILayout.PropertyField(sideProp, new GUIContent("Target Side"));

        // 6) Waifu Position
        //    Siempre visible luego.
        var posProp = prop.FindPropertyRelative("waifuPosition");
        if (posProp != null)
            EditorGUILayout.PropertyField(posProp, new GUIContent("Waifu Position"));

        // 7) Dynamic Filters
        //    Uno por cada EnchantFilterType que aún no hayas usado.
        var filtersProp = prop.FindPropertyRelative("filters");
        if (filtersProp != null && filtersProp.isArray)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Filters", EditorStyles.boldLabel);

            // — Mostrar filtros actuales con botón “✕”
            int removeIdx = -1;
            for (int i = 0; i < filtersProp.arraySize; i++)
            {
                var entry = filtersProp.GetArrayElementAtIndex(i);
                var ftProp = entry.FindPropertyRelative("filterType");
                if (ftProp == null) continue;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(ftProp, GUIContent.none, GUILayout.Width(100));

                switch ((EnchantFilterType)ftProp.enumValueIndex)
                {
                    case EnchantFilterType.Role:
                        var roleProp = entry.FindPropertyRelative("roleFilter");
                        if (roleProp != null) EditorGUILayout.PropertyField(roleProp, GUIContent.none);
                        break;
                    case EnchantFilterType.Reign:
                        var reignProp = entry.FindPropertyRelative("reignFilter");
                        if (reignProp != null) EditorGUILayout.PropertyField(reignProp, GUIContent.none);
                        break;
                    case EnchantFilterType.Element:
                        var elemProp = entry.FindPropertyRelative("elementFilter");
                        if (elemProp != null) EditorGUILayout.PropertyField(elemProp, GUIContent.none);
                        break;
                }

                if (GUILayout.Button("✕", GUILayout.Width(20)))
                    removeIdx = i;
                EditorGUILayout.EndHorizontal();
            }

            if (removeIdx >= 0)
                filtersProp.DeleteArrayElementAtIndex(removeIdx);

            // — “+ Add Filter” para el primer tipo disponible —
            var used = Enumerable.Range(0, filtersProp.arraySize)
                .Select(i => {
                    var e = filtersProp.GetArrayElementAtIndex(i)
                                      .FindPropertyRelative("filterType");
                    return e != null ? (EnchantFilterType)e.enumValueIndex : (EnchantFilterType?)null;
                })
                .Where(x => x.HasValue)
                .Select(x => x.Value)
                .ToList();

            var allTypes = Enum.GetValues(typeof(EnchantFilterType)).Cast<EnchantFilterType>();
            var available = allTypes.Except(used).ToList();

            if (available.Count > 0 && GUILayout.Button("+ Add Filter"))
            {
                filtersProp.InsertArrayElementAtIndex(filtersProp.arraySize);
                var newEntry = filtersProp.GetArrayElementAtIndex(filtersProp.arraySize - 1);

                var ftNew = newEntry.FindPropertyRelative("filterType");
                if (ftNew != null) ftNew.enumValueIndex = (int)available[0];

                // inicializar subcampos
                var rNew = newEntry.FindPropertyRelative("roleFilter");
                if (rNew != null) rNew.enumValueIndex = 0;
                var reignNew = newEntry.FindPropertyRelative("reignFilter");
                if (reignNew != null) reignNew.enumValueIndex = 0;
                var elNew = newEntry.FindPropertyRelative("elementFilter");
                if (elNew != null) elNew.enumValueIndex = 0;
            }
        }
    }
}
#endif
