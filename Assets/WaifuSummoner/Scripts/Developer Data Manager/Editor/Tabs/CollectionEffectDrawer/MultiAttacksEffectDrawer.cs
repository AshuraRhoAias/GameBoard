#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[EffectDrawer(EffectType.MultiAttacks, "multiAttacksEffect")]
public class MultiAttacksEffectDrawer : IEffectDrawer
{
    public void Draw(SerializedProperty prop)
    {
        if (prop == null) return;

        // 1) Targets (None/All/Select/Random/Situational/Self)
        var targetProp = prop.FindPropertyRelative("target");
        EditorGUILayout.PropertyField(targetProp, new GUIContent("Targets"));
        var target = (Target)targetProp.enumValueIndex;
        if (target == Target.None) return;

        // 2) Target Side — sólo si el target lo requiere (no para Self)
        if (target != Target.Self)
        {
            EditorGUILayout.PropertyField(
                prop.FindPropertyRelative("targetSide"),
                new GUIContent("Target Side")
            );
        }

        // 3) Attack Mode (NumberOfTimes/NumberVsWaifus/AllWaifus)
        var modeProp = prop.FindPropertyRelative("multiAttackType");
        EditorGUILayout.PropertyField(modeProp, new GUIContent("Attack Mode"));
        var mode = (MultiAttackTypes)modeProp.enumValueIndex;
        if (mode == MultiAttackTypes.None) return;

        // 4) Count — sólo si el modo lo necesita
        if (mode == MultiAttackTypes.NumberOfTimes
         || mode == MultiAttackTypes.NumberVsWaifus)
        {
            var amtProp = prop.FindPropertyRelative("amount");
            amtProp.intValue = Mathf.Max(1, amtProp.intValue);
            EditorGUILayout.PropertyField(amtProp, new GUIContent("Number of Times"));
        }

        // 5) Situational extras — sólo si Targets == Situational
        if (target == Target.Situational)
        {
            EditorGUILayout.PropertyField(
                prop.FindPropertyRelative("highLow"),
                new GUIContent("Highest / Lowest")
            );
            EditorGUILayout.PropertyField(
                prop.FindPropertyRelative("situationalStat"),
                new GUIContent("Stat to Compare")
            );
        }

        // 6) Duration
        EditorGUILayout.PropertyField(
            prop.FindPropertyRelative("duration"),
            new GUIContent("Duration")
        );
        var duration = (Duration)prop.FindPropertyRelative("duration").enumValueIndex;
        if (duration == Duration.UntilTheNext)
        {
            EditorGUILayout.PropertyField(
                prop.FindPropertyRelative("untilStage"),
                new GUIContent("Until Stage")
            );
            var turnsProp = prop.FindPropertyRelative("durationTurns");
            turnsProp.intValue = Mathf.Max(1, turnsProp.intValue);
            EditorGUILayout.PropertyField(turnsProp, new GUIContent("Turns"));
        }
        else if (duration == Duration.ForNumberTurns
              || duration == Duration.ForNumberOfYourTurns)
        {
            var turnsProp = prop.FindPropertyRelative("durationTurns");
            turnsProp.intValue = Mathf.Max(1, turnsProp.intValue);
            EditorGUILayout.PropertyField(turnsProp, new GUIContent("Turns"));
        }

        // 7) Dynamic Filters
        var filtersProp = prop.FindPropertyRelative("filters");
        if (filtersProp != null && filtersProp.isArray)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Filters", EditorStyles.boldLabel);

            // Recolectar tipos ya usados
            var used = Enumerable.Range(0, filtersProp.arraySize)
                .Select(i => (MultiAttackFilterType)filtersProp
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
                switch ((MultiAttackFilterType)ftProp.enumValueIndex)
                {
                    case MultiAttackFilterType.SummonCondition:
                        EditorGUILayout.PropertyField(
                            entry.FindPropertyRelative("summonCondition"),
                            GUIContent.none);
                        break;
                    case MultiAttackFilterType.Role:
                        EditorGUILayout.PropertyField(
                            entry.FindPropertyRelative("roleFilter"),
                            GUIContent.none);
                        break;
                    case MultiAttackFilterType.Element:
                        EditorGUILayout.PropertyField(
                            entry.FindPropertyRelative("elementFilter"),
                            GUIContent.none);
                        break;
                    case MultiAttackFilterType.Reign:
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

            // “+ Add Filter” mientras queden tipos libres
            var allTypes = Enum.GetValues(typeof(MultiAttackFilterType))
                               .Cast<MultiAttackFilterType>();
            var available = allTypes.Except(used).ToList();
            if (available.Count > 0 && GUILayout.Button("+ Add Filter"))
            {
                filtersProp.InsertArrayElementAtIndex(filtersProp.arraySize);
                var newEntry = filtersProp.GetArrayElementAtIndex(filtersProp.arraySize - 1);

                newEntry.FindPropertyRelative("filterType").enumValueIndex = (int)available[0];
                newEntry.FindPropertyRelative("summonCondition").enumValueIndex = 0;
                newEntry.FindPropertyRelative("roleFilter").enumValueIndex = 0;
                newEntry.FindPropertyRelative("elementFilter").enumValueIndex = 0;
                newEntry.FindPropertyRelative("reignFilter").enumValueIndex = 0;
            }
        }
    }
}
#endif
