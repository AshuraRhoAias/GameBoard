// Assets/WaifuSummoner/Scripts/Developer Data Manager/Editor/Tabs/CollectionEffectDrawers/ProtectionEffectDrawer.cs
#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[EffectDrawer(EffectType.Protection, "protectionEffect")]
public class ProtectionEffectDrawer : IEffectDrawer
{
    // 1) Qué opciones permitir por cada ProtectionTarget
    private static ProtectionOption[] GetAllowedOptions(ProtectionTarget target)
    {
        switch (target)
        {
            case ProtectionTarget.AnyCard:
            case ProtectionTarget.Enchantment:
            case ProtectionTarget.EnchantmentMood:
            case ProtectionTarget.Mood:
                return new[]
                {
                    ProtectionOption.EffectIndestructible,
                    ProtectionOption.CannotBeTargeted
                };

            case ProtectionTarget.Waifu:
                return new[]
                {
                    ProtectionOption.BattleIndestructible,
                    ProtectionOption.EffectIndestructible,
                    ProtectionOption.CannotBeTargeted,
                    ProtectionOption.PreventBattleDamage
                };

            case ProtectionTarget.Opponent:
            case ProtectionTarget.User:
            case ProtectionTarget.OpponentAndUser:
                return new[]
                {
                    ProtectionOption.LibidoDamageImmunity,
                    ProtectionOption.HandManipulationImmunity,
                    ProtectionOption.DeckManipulationImmunity
                };

            default:
                return Array.Empty<ProtectionOption>();
        }
    }

    public void Draw(SerializedProperty prop)
    {
        if (prop == null) return;

        // — 1) ProtectionTarget —
        var tp = prop.FindPropertyRelative("protectionTarget");
        EditorGUILayout.PropertyField(tp, new GUIContent("Protect"));
        var protTarget = (ProtectionTarget)tp.enumValueIndex;
        if (protTarget == ProtectionTarget.None) return;

        // — 2) ProtectionOptions (filtradas) —
        var opts = prop.FindPropertyRelative("options");
        if (opts.isArray)
        {
            EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
            var allowed = GetAllowedOptions(protTarget);

            // collect used
            var usedOpts = Enumerable.Range(0, opts.arraySize)
                                     .Select(i => (ProtectionOption)opts.GetArrayElementAtIndex(i).enumValueIndex)
                                     .ToList();

            int removeOpt = -1;
            for (int i = 0; i < opts.arraySize; i++)
            {
                var elemProp = opts.GetArrayElementAtIndex(i);
                var current = (ProtectionOption)elemProp.enumValueIndex;

                // build choice list = allowed + current
                var choices = allowed
                    .Union(new[] { current })
                    .Distinct()
                    .ToArray();

                var labels = choices.Select(e => e.ToString()).ToArray();
                int currentIndex = Array.IndexOf(choices, current);

                EditorGUILayout.BeginHorizontal();
                int newIndex = EditorGUILayout.Popup(currentIndex, labels);
                elemProp.enumValueIndex = (int)choices[newIndex];
                if (GUILayout.Button("✕", GUILayout.Width(20)))
                    removeOpt = i;
                EditorGUILayout.EndHorizontal();
            }

            if (removeOpt >= 0)
                opts.DeleteArrayElementAtIndex(removeOpt);

            // "+ Add Option"
            var avail = allowed.Except(usedOpts).ToList();
            if (avail.Count > 0 && GUILayout.Button("+ Add Option"))
            {
                opts.InsertArrayElementAtIndex(opts.arraySize);
                opts.GetArrayElementAtIndex(opts.arraySize - 1).enumValueIndex = (int)avail[0];
            }
        }

        // — 3) Si protegemos cartas, abrimos selector de cartas —
        bool isCard = protTarget == ProtectionTarget.AnyCard
                   || protTarget == ProtectionTarget.Waifu
                   || protTarget == ProtectionTarget.Enchantment
                   || protTarget == ProtectionTarget.EnchantmentMood
                   || protTarget == ProtectionTarget.Mood;

        if (isCard)
        {
            // 3.1) Card Targets
            var ct = prop.FindPropertyRelative("cardTarget");
            EditorGUILayout.PropertyField(ct, new GUIContent("Card Targets"));
            var cardTarget = (Target)ct.enumValueIndex;
            if (cardTarget == Target.None) return;

            // 3.2) Target Side
            EditorGUILayout.PropertyField(
                prop.FindPropertyRelative("targetSide"),
                new GUIContent("Target Side")
            );

            // 3.3) Amount
            if (cardTarget == Target.Select
             || cardTarget == Target.Random
             || cardTarget == Target.Situational)
            {
                var amt = prop.FindPropertyRelative("amount");
                amt.intValue = Mathf.Max(1, amt.intValue);
                EditorGUILayout.PropertyField(amt, new GUIContent("Amount"));
            }

            // 3.4) Situational extras
            if (cardTarget == Target.Situational)
            {
                EditorGUILayout.PropertyField(
                    prop.FindPropertyRelative("highLow"),
                    new GUIContent("Highest / Lowest")
                );
                EditorGUILayout.PropertyField(
                    prop.FindPropertyRelative("situationalStat"),
                    new GUIContent("Stat to Compare")
                );
                EditorGUILayout.PropertyField(
                    prop.FindPropertyRelative("tieBreaker"),
                    new GUIContent("Tie Breaker")
                );
            }

            // — 4) Filters dinámicos (solo Waifu) —
            if (protTarget == ProtectionTarget.Waifu)
            {
                var wf = prop.FindPropertyRelative("filters");
                if (wf.isArray)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Waifu Filters", EditorStyles.boldLabel);

                    var usedF = new List<ProtectionFilterType>();
                    for (int i = 0; i < wf.arraySize; i++)
                    {
                        var entry = wf.GetArrayElementAtIndex(i);
                        var ftProp = entry.FindPropertyRelative("filterType");
                        usedF.Add((ProtectionFilterType)ftProp.enumValueIndex);
                    }

                    int removeF = -1;
                    for (int i = 0; i < wf.arraySize; i++)
                    {
                        var entry = wf.GetArrayElementAtIndex(i);
                        var ftProp = entry.FindPropertyRelative("filterType");
                        var current = (ProtectionFilterType)ftProp.enumValueIndex;

                        // build filter choices
                        var allF = Enum.GetValues(typeof(ProtectionFilterType))
                                       .Cast<ProtectionFilterType>();
                        var choicesF = allF
                            .Except(usedF.Where((_, idx) => idx != i))
                            .ToArray();
                        var labelsF = choicesF.Select(e => e.ToString()).ToArray();
                        int idxF = Array.IndexOf(choicesF, current);

                        EditorGUILayout.BeginHorizontal();
                        int newF = EditorGUILayout.Popup(idxF, labelsF, GUILayout.Width(100));
                        ftProp.enumValueIndex = (int)choicesF[newF];

                        // draw the value field
                        switch ((ProtectionFilterType)ftProp.enumValueIndex)
                        {
                            case ProtectionFilterType.SummonCondition:
                                EditorGUILayout.PropertyField(entry.FindPropertyRelative("summonCondition"), GUIContent.none);
                                break;
                            case ProtectionFilterType.Role:
                                EditorGUILayout.PropertyField(entry.FindPropertyRelative("roleFilter"), GUIContent.none);
                                break;
                            case ProtectionFilterType.Element:
                                EditorGUILayout.PropertyField(entry.FindPropertyRelative("elementFilter"), GUIContent.none);
                                break;
                            case ProtectionFilterType.Reign:
                                EditorGUILayout.PropertyField(entry.FindPropertyRelative("reignFilter"), GUIContent.none);
                                break;
                        }

                        if (GUILayout.Button("✕", GUILayout.Width(20)))
                            removeF = i;
                        EditorGUILayout.EndHorizontal();
                    }

                    if (removeF >= 0)
                        wf.DeleteArrayElementAtIndex(removeF);

                    var availF = Enum.GetValues(typeof(ProtectionFilterType))
                                     .Cast<ProtectionFilterType>()
                                     .Except(usedF)
                                     .ToList();
                    if (availF.Count > 0 && GUILayout.Button("+ Add Filter"))
                    {
                        wf.InsertArrayElementAtIndex(wf.arraySize);
                        var ne = wf.GetArrayElementAtIndex(wf.arraySize - 1);
                        ne.FindPropertyRelative("filterType").enumValueIndex = (int)availF[0];
                        ne.FindPropertyRelative("summonCondition").enumValueIndex = 0;
                        ne.FindPropertyRelative("roleFilter").enumValueIndex = 0;
                        ne.FindPropertyRelative("elementFilter").enumValueIndex = 0;
                        ne.FindPropertyRelative("reignFilter").enumValueIndex = 0;
                    }
                }
            }
        }

        // — 5) Duración (siempre) —
        EditorGUILayout.Space();
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
            var t = prop.FindPropertyRelative("durationTurns");
            t.intValue = Mathf.Max(1, t.intValue);
            EditorGUILayout.PropertyField(t, new GUIContent("Turns"));
        }
        else if (dur == Duration.ForNumberTurns || dur == Duration.ForNumberOfYourTurns)
        {
            var t2 = prop.FindPropertyRelative("durationTurns");
            t2.intValue = Mathf.Max(1, t2.intValue);
            EditorGUILayout.PropertyField(t2, new GUIContent("Turns"));
        }
    }
}
#endif
