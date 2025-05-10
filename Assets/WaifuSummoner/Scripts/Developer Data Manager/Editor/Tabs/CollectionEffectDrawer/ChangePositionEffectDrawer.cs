// Assets/WaifuSummoner/Scripts/Developer Data Manager/Editor/Drawers/ChangePositionEffectDrawer.cs
#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[EffectDrawer(EffectType.ChangePosition, "changePositionEffect")]
public class ChangePositionEffectDrawer : IEffectDrawer
{
    public void Draw(SerializedProperty prop)
    {
        if (prop == null) return;

        // 0) Nueva posición
        var posProp = prop.FindPropertyRelative("newPosition");
        EditorGUILayout.PropertyField(posProp, new GUIContent("New Position"));

        // 1) Targets
        var targetProp = prop.FindPropertyRelative("target");
        EditorGUILayout.PropertyField(targetProp, new GUIContent("Targets"));
        var target = (Target)targetProp.enumValueIndex;
        if (target == Target.None) return;

        // 2) Target Side (no aplica si es Self)
        if (target != Target.Self)
        {
            EditorGUILayout.PropertyField(
                prop.FindPropertyRelative("targetSide"),
                new GUIContent("Target Side")
            );
        }

        // 3) Amount (solo Select/Random/Situational)
        if (target == Target.Select
         || target == Target.Random
         || target == Target.Situational)
        {
            var amtProp = prop.FindPropertyRelative("amount");
            amtProp.intValue = Mathf.Max(1, amtProp.intValue);
            EditorGUILayout.PropertyField(amtProp, new GUIContent("Amount"));
        }

        // 4) Situational extras
        if (target == Target.Situational)
        {
            EditorGUILayout.PropertyField(
                prop.FindPropertyRelative("highLow"),
                new GUIContent("Highest / Lowest")
            );
            EditorGUILayout.PropertyField(
                prop.FindPropertyRelative("statType"),
                new GUIContent("Stat to Compare")
            );
            EditorGUILayout.PropertyField(
                prop.FindPropertyRelative("tieBreaker"),
                new GUIContent("Tie Breaker")
            );
        }

        // 5) Duration
        EditorGUILayout.PropertyField(
            prop.FindPropertyRelative("duration"),
            new GUIContent("Duration")
        );
        var dur = (Duration)prop.FindPropertyRelative("duration").enumValueIndex;
        if (dur == Duration.UntilTheNext)
        {
            EditorGUILayout.PropertyField(
                prop.FindPropertyRelative("untilStage"),
                new GUIContent("Until Stage")
            );
            var turnsU = prop.FindPropertyRelative("durationTurns");
            turnsU.intValue = Mathf.Max(1, turnsU.intValue);
            EditorGUILayout.PropertyField(turnsU, new GUIContent("Turns"));
        }
        else if (dur == Duration.ForNumberTurns
              || dur == Duration.ForNumberOfYourTurns)
        {
            var turns = prop.FindPropertyRelative("durationTurns");
            turns.intValue = Mathf.Max(1, turns.intValue);
            EditorGUILayout.PropertyField(turns, new GUIContent("Turns"));
        }

        // 6) Dynamic Filters
        var filtersProp = prop.FindPropertyRelative("filters");
        if (filtersProp != null && filtersProp.isArray)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Filters", EditorStyles.boldLabel);

            var used = Enumerable.Range(0, filtersProp.arraySize)
                .Select(i => (ChangePositionFilterType)filtersProp
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

                switch ((ChangePositionFilterType)ftProp.enumValueIndex)
                {
                    case ChangePositionFilterType.SummonCondition:
                        EditorGUILayout.PropertyField(
                            entry.FindPropertyRelative("summonCondition"),
                            GUIContent.none);
                        break;
                    case ChangePositionFilterType.Role:
                        EditorGUILayout.PropertyField(
                            entry.FindPropertyRelative("roleFilter"),
                            GUIContent.none);
                        break;
                    case ChangePositionFilterType.Element:
                        EditorGUILayout.PropertyField(
                            entry.FindPropertyRelative("elementFilter"),
                            GUIContent.none);
                        break;
                    case ChangePositionFilterType.Reign:
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

            var allTypes = Enum.GetValues(typeof(ChangePositionFilterType))
                               .Cast<ChangePositionFilterType>();
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
