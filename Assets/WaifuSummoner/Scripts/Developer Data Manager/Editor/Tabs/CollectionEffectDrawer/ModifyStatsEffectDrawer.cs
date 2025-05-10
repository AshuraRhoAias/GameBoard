#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[EffectDrawer(EffectType.ModifyStats, "modifyStatsEffect")]
public class ModifyStatsEffectDrawer : IEffectDrawer
{
    public void Draw(SerializedProperty mProp)
    {
        if (mProp == null) return;

        // 1) Stat
        EditorGUILayout.PropertyField(
            mProp.FindPropertyRelative("stat"),
            new GUIContent("Stat")
        );

        // 2) Action
        EditorGUILayout.PropertyField(
            mProp.FindPropertyRelative("action"),
            new GUIContent("Action")
        );

        // 3) Value
        var valueProp = mProp.FindPropertyRelative("value");
        valueProp.intValue = Mathf.Max(1, valueProp.intValue);
        EditorGUILayout.PropertyField(valueProp, new GUIContent("Value"));

        // 4) Targets
        var tgtProp = mProp.FindPropertyRelative("targetType");
        EditorGUILayout.PropertyField(tgtProp, new GUIContent("Targets"));
        var tt = (Target)tgtProp.enumValueIndex;
        if (tt == Target.None) return;

        // 5) Count (solo Select/Random/Situational)
        if (tt == Target.Select || tt == Target.Random || tt == Target.Situational)
        {
            var amtProp = mProp.FindPropertyRelative("amount");
            amtProp.intValue = Mathf.Max(1, amtProp.intValue);
            EditorGUILayout.PropertyField(amtProp, new GUIContent("Count"));
        }

        // 6) Situational extras
        if (tt == Target.Situational)
        {
            EditorGUILayout.PropertyField(
                mProp.FindPropertyRelative("highLow"),
                new GUIContent("Highest / Lowest")
            );
            EditorGUILayout.PropertyField(
                mProp.FindPropertyRelative("situationalStat"),
                new GUIContent("Stat to Compare")
            );
            EditorGUILayout.PropertyField(
                mProp.FindPropertyRelative("tieBreaker"),
                new GUIContent("Tie Breaker")
            );
        }

        // 7) Target Side (¡no para Self!)
        if (tt != Target.Self)
        {
            EditorGUILayout.PropertyField(
                mProp.FindPropertyRelative("targetSide"),
                new GUIContent("Target Side")
            );
        }

        // 8) Duration
        var durProp = mProp.FindPropertyRelative("duration");
        EditorGUILayout.PropertyField(durProp, new GUIContent("Duration"));
        var dur = (Duration)durProp.enumValueIndex;
        if (dur == Duration.ForNumberTurns || dur == Duration.ForNumberOfYourTurns)
        {
            var turnsProp = mProp.FindPropertyRelative("durationTurns");
            turnsProp.intValue = Mathf.Max(1, turnsProp.intValue);
            string label = dur == Duration.ForNumberTurns ? "Turns" : "Your Turns";
            EditorGUILayout.PropertyField(turnsProp, new GUIContent($"  {label}"));
        }

        // 9) Dynamic Filters (no para Self tampoco)
        if (tt != Target.Self)
        {
            var filtersProp = mProp.FindPropertyRelative("filters");
            if (filtersProp != null && filtersProp.isArray)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Filters", EditorStyles.boldLabel);

                var used = Enumerable.Range(0, filtersProp.arraySize)
                    .Select(i => (ModifyStatsFilterType)filtersProp
                        .GetArrayElementAtIndex(i)
                        .FindPropertyRelative("filterType")
                        .enumValueIndex)
                    .ToList();

                int removeIdx = -1;
                for (int i = 0; i < filtersProp.arraySize; i++)
                {
                    var entry = filtersProp.GetArrayElementAtIndex(i);
                    var ftProp = entry.FindPropertyRelative("filterType");
                    if (ftProp == null) continue;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(ftProp, GUIContent.none, GUILayout.Width(100));
                    switch ((ModifyStatsFilterType)ftProp.enumValueIndex)
                    {
                        case ModifyStatsFilterType.SummonCondition:
                            EditorGUILayout.PropertyField(
                                entry.FindPropertyRelative("summonConditionFilter"),
                                GUIContent.none
                            );
                            break;
                        case ModifyStatsFilterType.Role:
                            EditorGUILayout.PropertyField(
                                entry.FindPropertyRelative("roleFilter"),
                                GUIContent.none
                            );
                            break;
                        case ModifyStatsFilterType.Element:
                            EditorGUILayout.PropertyField(
                                entry.FindPropertyRelative("elementFilter"),
                                GUIContent.none
                            );
                            break;
                        case ModifyStatsFilterType.Reign:
                            EditorGUILayout.PropertyField(
                                entry.FindPropertyRelative("reignFilter"),
                                GUIContent.none
                            );
                            break;
                    }
                    if (GUILayout.Button("✕", GUILayout.Width(20)))
                        removeIdx = i;
                    EditorGUILayout.EndHorizontal();
                }

                if (removeIdx >= 0)
                    filtersProp.DeleteArrayElementAtIndex(removeIdx);

                var allTypes = Enum.GetValues(typeof(ModifyStatsFilterType)).Cast<ModifyStatsFilterType>();
                var available = allTypes.Except(used).ToList();
                if (available.Count > 0 && GUILayout.Button("+ Add Filter"))
                {
                    filtersProp.InsertArrayElementAtIndex(filtersProp.arraySize);
                    var newEntry = filtersProp.GetArrayElementAtIndex(filtersProp.arraySize - 1);

                    var ftNew = newEntry.FindPropertyRelative("filterType");
                    if (ftNew != null) ftNew.enumValueIndex = (int)available[0];

                    newEntry.FindPropertyRelative("summonConditionFilter").enumValueIndex = 0;
                    newEntry.FindPropertyRelative("roleFilter").enumValueIndex = 0;
                    newEntry.FindPropertyRelative("elementFilter").enumValueIndex = 0;
                    newEntry.FindPropertyRelative("reignFilter").enumValueIndex = 0;
                }
            }
        }
    }
}
#endif
