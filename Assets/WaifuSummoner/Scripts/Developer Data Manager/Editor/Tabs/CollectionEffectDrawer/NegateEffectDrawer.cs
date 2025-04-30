#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[EffectDrawer(EffectType.NegateEffect, "negateEffect")]
public class NegateEffectDrawer : IEffectDrawer
{
    public void Draw(SerializedProperty prop)
    {
        if (prop == null) return;

        // — 0) Field Card Type —
        var fieldProp = prop.FindPropertyRelative("fieldCardType");
        EditorGUILayout.PropertyField(fieldProp, new GUIContent("Field Card Type"));

        // — 1) Target —
        var tgtProp = prop.FindPropertyRelative("target");
        EditorGUILayout.PropertyField(tgtProp, new GUIContent("Target"));
        var tgt = (Target)tgtProp.enumValueIndex;
        if (tgt == Target.None) return;

        // — 2) Amount (Select/Random/Situational) —
        if (tgt == Target.Select || tgt == Target.Random || tgt == Target.Situational)
        {
            var amtProp = prop.FindPropertyRelative("amount");
            amtProp.intValue = Mathf.Max(1, amtProp.intValue);
            EditorGUILayout.PropertyField(amtProp, new GUIContent("Amount"));
        }

        // — 3) Situational extras —
        if (tgt == Target.Situational)
        {
            var hlProp = prop.FindPropertyRelative("highLow");
            var statProp = prop.FindPropertyRelative("statType");
            EditorGUILayout.PropertyField(hlProp, new GUIContent("Highest / Lowest"));
            EditorGUILayout.PropertyField(statProp, new GUIContent("Stat to Compare"));
        }

        // — 4) Target Side —
        var sideProp = prop.FindPropertyRelative("targetSide");
        EditorGUILayout.PropertyField(sideProp, new GUIContent("Target Side"));

        // — 5) Duration —
        var durProp = prop.FindPropertyRelative("duration");
        EditorGUILayout.PropertyField(durProp, new GUIContent("Duration"));
        var dur = (Duration)durProp.enumValueIndex;

        if (dur == Duration.UntilTheNext)
        {
            var stageProp = prop.FindPropertyRelative("untilStage");
            EditorGUILayout.PropertyField(stageProp, new GUIContent("Until Stage"));
            var turnsProp = prop.FindPropertyRelative("durationTurns");
            turnsProp.intValue = Mathf.Max(1, turnsProp.intValue);
            EditorGUILayout.PropertyField(turnsProp, new GUIContent("Turns"));
        }
        else if (dur == Duration.ForNumberTurns || dur == Duration.ForNumberOfYourTurns)
        {
            var turnsProp = prop.FindPropertyRelative("durationTurns");
            turnsProp.intValue = Mathf.Max(1, turnsProp.intValue);
            EditorGUILayout.PropertyField(turnsProp, new GUIContent("Turns"));
        }

        // — 6) Dynamic Filters —
        var filtersProp = prop.FindPropertyRelative("filters");
        if (filtersProp != null && filtersProp.isArray)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Filters", EditorStyles.boldLabel);

            // already-used types
            var used = Enumerable.Range(0, filtersProp.arraySize)
                .Select(i => {
                    var e = filtersProp.GetArrayElementAtIndex(i)
                                      .FindPropertyRelative("filterType");
                    return e != null ? (NegateFilterType?)e.enumValueIndex : null;
                })
                .Where(x => x.HasValue)
                .Select(x => x.Value)
                .ToList();

            int removeIdx = -1;
            for (int i = 0; i < filtersProp.arraySize; i++)
            {
                var entry = filtersProp.GetArrayElementAtIndex(i);
                var ftProp = entry.FindPropertyRelative("filterType");
                if (ftProp == null) continue;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(ftProp, GUIContent.none, GUILayout.Width(100));

                switch ((NegateFilterType)ftProp.enumValueIndex)
                {
                    case NegateFilterType.SummonCondition:
                        EditorGUILayout.PropertyField(
                            entry.FindPropertyRelative("summonCondition"),
                            GUIContent.none);
                        break;
                    case NegateFilterType.Role:
                        EditorGUILayout.PropertyField(
                            entry.FindPropertyRelative("roleFilter"),
                            GUIContent.none);
                        break;
                    case NegateFilterType.Element:
                        EditorGUILayout.PropertyField(
                            entry.FindPropertyRelative("elementFilter"),
                            GUIContent.none);
                        break;
                    case NegateFilterType.Reign:
                        EditorGUILayout.PropertyField(
                            entry.FindPropertyRelative("reignFilter"),
                            GUIContent.none);
                        break;
                }

                if (GUILayout.Button("✕", GUILayout.Width(20)))
                    removeIdx = i;
                EditorGUILayout.EndHorizontal();
            }

            if (removeIdx >= 0)
                filtersProp.DeleteArrayElementAtIndex(removeIdx);

            // + Add Filter
            var allTypes = Enum.GetValues(typeof(NegateFilterType)).Cast<NegateFilterType>();
            var available = allTypes.Except(used).ToList();
            if (available.Count > 0 && GUILayout.Button("+ Add Filter"))
            {
                filtersProp.InsertArrayElementAtIndex(filtersProp.arraySize);
                var newEntry = filtersProp.GetArrayElementAtIndex(filtersProp.arraySize - 1);

                var ftNew = newEntry.FindPropertyRelative("filterType");
                if (ftNew != null) ftNew.enumValueIndex = (int)available[0];

                newEntry.FindPropertyRelative("summonCondition").enumValueIndex = 0;
                newEntry.FindPropertyRelative("roleFilter").enumValueIndex = 0;
                newEntry.FindPropertyRelative("elementFilter").enumValueIndex = 0;
                newEntry.FindPropertyRelative("reignFilter").enumValueIndex = 0;
            }
        }
    }
}
#endif
