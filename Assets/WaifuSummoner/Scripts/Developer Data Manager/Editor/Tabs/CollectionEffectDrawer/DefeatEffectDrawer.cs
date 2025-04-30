#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Drawer para DefeatEffectData.
/// Se registra automáticamente para EffectType.Defeat y la propiedad "defeatEffect".
/// </summary>
[EffectDrawer(EffectType.Defeat, "defeatEffect")]
public class DefeatEffectDrawer : IEffectDrawer
{
    /// <summary>
    /// Se invoca desde CollectionTab: subProp ya apunta a la propiedad "defeatEffect"
    /// </summary>
    /// <param name="defProp">SerializedProperty de tu DefeatEffectData</param>
    public void Draw(SerializedProperty defProp)
    {
        if (defProp == null) return;

        // — 1) referencias —
        var targetTypeProp = defProp.FindPropertyRelative("targetType");
        var targetSideProp = defProp.FindPropertyRelative("targetSide");
        var amountProp = defProp.FindPropertyRelative("amount");
        var filtersProp = defProp.FindPropertyRelative("filters");
        var hlProp = defProp.FindPropertyRelative("situationalHighLow");
        var statProp = defProp.FindPropertyRelative("situationalStat");
        var tieProp = defProp.FindPropertyRelative("situationalTieBreaker");

        // — 2) Target Type —
        EditorGUILayout.PropertyField(targetTypeProp, new GUIContent("Target Type"));
        var tt = (Target)targetTypeProp.enumValueIndex;
        if (tt == Target.None)
            return;

        // — 3) Target Side —
        EditorGUILayout.PropertyField(targetSideProp, new GUIContent("Target Side"));

        // — 4) Amount (solo si aplica) —
        if (tt == Target.Select
         || tt == Target.Random
         || tt == Target.Situational)
        {
            if (amountProp.intValue < 1) amountProp.intValue = 1;
            EditorGUILayout.PropertyField(amountProp, new GUIContent("Amount"));
        }

        // — 5) Situational extras —
        if (tt == Target.Situational)
        {
            EditorGUILayout.PropertyField(hlProp, new GUIContent("Highest / Lowest"));
            EditorGUILayout.PropertyField(statProp, new GUIContent("Stat Type"));
            EditorGUILayout.PropertyField(tieProp, new GUIContent("Tie Breaker"));
        }

        // — 6) Filtros dinámicos —
        EditorGUILayout.LabelField("Filters", EditorStyles.boldLabel);

        // detecta ya usados para no duplicar
        var used = Enumerable.Range(0, filtersProp.arraySize)
            .Select(i => (DefeatFilterType)filtersProp
                .GetArrayElementAtIndex(i)
                .FindPropertyRelative("filterType")
                .enumValueIndex)
            .ToList();

        int removeIdx = -1;
        for (int i = 0; i < filtersProp.arraySize; i++)
        {
            var fProp = filtersProp.GetArrayElementAtIndex(i);
            var fTypeProp = fProp.FindPropertyRelative("filterType");

            EditorGUILayout.BeginHorizontal();
            // selector de tipo de filtro
            EditorGUILayout.PropertyField(fTypeProp, GUIContent.none, GUILayout.Width(100));

            // campo según tipo
            switch ((DefeatFilterType)fTypeProp.enumValueIndex)
            {
                case DefeatFilterType.SummonCondition:
                    EditorGUILayout.PropertyField(
                        fProp.FindPropertyRelative("summonConditionFilter"),
                        GUIContent.none
                    );
                    break;
                case DefeatFilterType.Role:
                    EditorGUILayout.PropertyField(
                        fProp.FindPropertyRelative("roleFilter"),
                        GUIContent.none
                    );
                    break;
                case DefeatFilterType.Element:
                    EditorGUILayout.PropertyField(
                        fProp.FindPropertyRelative("elementFilter"),
                        GUIContent.none
                    );
                    break;
            }

            // botón eliminar
            if (GUILayout.Button("✕", GUILayout.Width(20)))
                removeIdx = i;
            EditorGUILayout.EndHorizontal();
        }

        // eliminar después de cerrar todos los horizontales
        if (removeIdx >= 0)
            filtersProp.DeleteArrayElementAtIndex(removeIdx);

        // — 7) “+ Add Filter” si quedan tipos libres —
        var allTypes = System.Enum.GetValues(typeof(DefeatFilterType)).Cast<DefeatFilterType>();
        var available = allTypes.Except(used).ToList();
        if (available.Count > 0 && GUILayout.Button("+ Add Filter"))
        {
            filtersProp.InsertArrayElementAtIndex(filtersProp.arraySize);
            var newF = filtersProp.GetArrayElementAtIndex(filtersProp.arraySize - 1);

            // inicializar subcampos
            newF.FindPropertyRelative("filterType").enumValueIndex = (int)available[0];
            newF.FindPropertyRelative("summonConditionFilter").enumValueIndex = 0;
            newF.FindPropertyRelative("roleFilter").enumValueIndex = 0;
            newF.FindPropertyRelative("elementFilter").enumValueIndex = 0;
        }
    }
}
#endif
