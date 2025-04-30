#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[EffectDrawer(EffectType.Control, "controlEffect")]
public class ControlEffectDrawer : IEffectDrawer
{
    public void Draw(SerializedProperty prop)
    {
        if (prop == null) return;

        // — 1) Control Action —
        var actionProp = prop.FindPropertyRelative("control");
        EditorGUILayout.PropertyField(actionProp, new GUIContent("Control Action"));
        var action = (Control)actionProp.enumValueIndex;
        if (action == Control.None) return;

        // — 2) Target —
        var targetProp = prop.FindPropertyRelative("target");
        EditorGUILayout.PropertyField(targetProp, new GUIContent("Target"));
        var target = (Target)targetProp.enumValueIndex;
        if (target == Target.None) return;

        // — 3) Amount (Select/Random/Situational) —
        if (target == Target.Select
         || target == Target.Random
         || target == Target.Situational)
        {
            var amtProp = prop.FindPropertyRelative("amount");
            amtProp.intValue = Mathf.Max(1, amtProp.intValue);
            EditorGUILayout.PropertyField(amtProp, new GUIContent("Amount"));
        }

        // — 4) Situational extras —
        if (target == Target.Situational)
        {
            var hlProp = prop.FindPropertyRelative("highLow");
            var statProp = prop.FindPropertyRelative("statType");
            EditorGUILayout.PropertyField(hlProp, new GUIContent("Highest / Lowest"));
            EditorGUILayout.PropertyField(statProp, new GUIContent("Stat to Compare"));
        }

        // — 5) Target Side —
        var sideProp = prop.FindPropertyRelative("targetSide");
        EditorGUILayout.PropertyField(sideProp, new GUIContent("Target Side"));

        // — 6) Duration —
        var durProp = prop.FindPropertyRelative("duration");
        EditorGUILayout.PropertyField(durProp, new GUIContent("Duration"));
        var dur = (Duration)durProp.enumValueIndex;

        // — 7) Extras para ciertas duraciones —
        if (dur == Duration.UntilTheNext)
        {
            var stageProp = prop.FindPropertyRelative("untilStage");
            EditorGUILayout.PropertyField(stageProp, new GUIContent("Until Stage"));

            var turnsProp = prop.FindPropertyRelative("durationTurns");
            turnsProp.intValue = Mathf.Max(1, turnsProp.intValue);
            EditorGUILayout.PropertyField(turnsProp, new GUIContent("Turns"));
        }
        else if (dur == Duration.ForNumberTurns
              || dur == Duration.ForNumberOfYourTurns)
        {
            var turnsProp = prop.FindPropertyRelative("durationTurns");
            turnsProp.intValue = Mathf.Max(1, turnsProp.intValue);
            EditorGUILayout.PropertyField(turnsProp, new GUIContent("Turns"));
        }

        // — 8) Dynamic Filters —
        var filtersProp = prop.FindPropertyRelative("filters");
        if (filtersProp != null && filtersProp.isArray)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Filters", EditorStyles.boldLabel);

            // 8a) qué tipos ya están en uso
            var used = Enumerable.Range(0, filtersProp.arraySize)
                .Select(i => {
                    var e = filtersProp
                        .GetArrayElementAtIndex(i)
                        .FindPropertyRelative("filterType");
                    return e != null ? (ControlFilterType?)e.enumValueIndex : null;
                })
                .Where(x => x.HasValue)
                .Select(x => x.Value)
                .ToList();

            int removeIdx = -1;
            // 8b) dibuja cada filtro con su UI y botón “✕”
            for (int i = 0; i < filtersProp.arraySize; i++)
            {
                var entry = filtersProp.GetArrayElementAtIndex(i);
                var ftProp = entry.FindPropertyRelative("filterType");
                if (ftProp == null) continue;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(ftProp, GUIContent.none, GUILayout.Width(100));

                switch ((ControlFilterType)ftProp.enumValueIndex)
                {
                    case ControlFilterType.SummonCondition:
                        EditorGUILayout.PropertyField(
                            entry.FindPropertyRelative("summonCondition"),
                            GUIContent.none
                        );
                        break;
                    case ControlFilterType.Role:
                        EditorGUILayout.PropertyField(
                            entry.FindPropertyRelative("roleFilter"),
                            GUIContent.none
                        );
                        break;
                    case ControlFilterType.Element:
                        EditorGUILayout.PropertyField(
                            entry.FindPropertyRelative("elementFilter"),
                            GUIContent.none
                        );
                        break;
                    case ControlFilterType.Reign:
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

            // 8c) “+ Add Filter” para los tipos que faltan
            var allTypes = Enum.GetValues(typeof(ControlFilterType)).Cast<ControlFilterType>();
            var available = allTypes.Except(used).ToList();
            if (available.Count > 0 && GUILayout.Button("+ Add Filter"))
            {
                filtersProp.InsertArrayElementAtIndex(filtersProp.arraySize);
                var newEntry = filtersProp.GetArrayElementAtIndex(filtersProp.arraySize - 1);

                var ftNew = newEntry.FindPropertyRelative("filterType");
                if (ftNew != null) ftNew.enumValueIndex = (int)available[0];

                // inicializar subcampos a 0
                foreach (var sub in new[] {
                    "summonCondition", "roleFilter", "elementFilter", "reignFilter"
                })
                {
                    var p = newEntry.FindPropertyRelative(sub);
                    if (p != null) p.enumValueIndex = 0;
                }
            }
        }
    }
}
#endif
