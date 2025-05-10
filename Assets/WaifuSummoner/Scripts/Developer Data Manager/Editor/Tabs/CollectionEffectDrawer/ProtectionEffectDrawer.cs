// Assets/WaifuSummoner/Scripts/Developer Data Manager/Editor/Tabs/CollectionEffectDrawers/ProtectionEffectDrawer.cs
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[EffectDrawer(EffectType.Protection, "protectionEffect")]
public class ProtectionEffectDrawer : IEffectDrawer
{
    // Define qué opciones están permitidas según el tipo de protección
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

        // 1) ProtectionTarget
        var protProp = prop.FindPropertyRelative("protectionTarget");
        if (protProp == null) return;
        EditorGUILayout.PropertyField(protProp, new GUIContent("Protect"));
        var protTarget = (ProtectionTarget)protProp.enumValueIndex;
        if (protTarget == ProtectionTarget.None) return;

        // 2) Protection Options (siempre visibles)
        var optsProp = prop.FindPropertyRelative("options");
        if (optsProp != null && optsProp.isArray)
        {
            EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
            var allowed = GetAllowedOptions(protTarget);
            var used = Enumerable.Range(0, optsProp.arraySize)
                                 .Select(i => (ProtectionOption)optsProp.GetArrayElementAtIndex(i).enumValueIndex)
                                 .ToList();

            int removeOpt = -1;
            for (int i = 0; i < optsProp.arraySize; i++)
            {
                var elem = optsProp.GetArrayElementAtIndex(i);
                var current = (ProtectionOption)elem.enumValueIndex;
                var choices = allowed.Union(new[] { current }).Distinct().ToArray();
                var labels = choices.Select(c => c.ToString()).ToArray();
                int idx = Array.IndexOf(choices, current);

                EditorGUILayout.BeginHorizontal();
                int sel = EditorGUILayout.Popup(idx, labels);
                elem.enumValueIndex = (int)choices[sel];
                if (GUILayout.Button("✕", GUILayout.Width(20)))
                    removeOpt = i;
                EditorGUILayout.EndHorizontal();
            }
            if (removeOpt >= 0)
                optsProp.DeleteArrayElementAtIndex(removeOpt);

            var avail = allowed.Except(used).ToList();
            if (avail.Count > 0 && GUILayout.Button("+ Add Option"))
            {
                optsProp.InsertArrayElementAtIndex(optsProp.arraySize);
                optsProp.GetArrayElementAtIndex(optsProp.arraySize - 1).enumValueIndex = (int)avail[0];
            }
        }

        // 3) Si protegemos cartas, mostramos Targets / Target Side / Amount / Situational
        bool isCard = protTarget == ProtectionTarget.AnyCard
                   || protTarget == ProtectionTarget.Waifu
                   || protTarget == ProtectionTarget.Enchantment
                   || protTarget == ProtectionTarget.EnchantmentMood
                   || protTarget == ProtectionTarget.Mood;
        if (isCard)
        {
            // 3.1) Card Targets
            var cardTgtProp = prop.FindPropertyRelative("cardTarget");
            if (cardTgtProp != null)
            {
                EditorGUILayout.PropertyField(cardTgtProp, new GUIContent("Targets"));
                var cardTgt = (Target)cardTgtProp.enumValueIndex;
                if (cardTgt == Target.None)
                    return;

                // 3.2) Target Side
                var sideProp = prop.FindPropertyRelative("targetSide");
                if (sideProp != null)
                    EditorGUILayout.PropertyField(sideProp, new GUIContent("Target Side"));

                // 3.3) Amount
                if (cardTgt == Target.Select || cardTgt == Target.Random || cardTgt == Target.Situational)
                {
                    var amtProp = prop.FindPropertyRelative("amount");
                    if (amtProp != null)
                    {
                        amtProp.intValue = Mathf.Max(1, amtProp.intValue);
                        EditorGUILayout.PropertyField(amtProp, new GUIContent("Amount"));
                    }
                }

                // 3.4) Situational extras
                if (cardTgt == Target.Situational)
                {
                    var hlProp = prop.FindPropertyRelative("highLow");
                    var stProp = prop.FindPropertyRelative("situationalStat");
                    var tbProp = prop.FindPropertyRelative("tieBreaker");
                    if (hlProp != null) EditorGUILayout.PropertyField(hlProp, new GUIContent("Highest / Lowest"));
                    if (stProp != null) EditorGUILayout.PropertyField(stProp, new GUIContent("Stat to Compare"));
                    if (tbProp != null) EditorGUILayout.PropertyField(tbProp, new GUIContent("Tie Breaker"));
                }
            }

            // 3.5) Waifu-only filters
            if (protTarget == ProtectionTarget.Waifu)
            {
                var filtProp = prop.FindPropertyRelative("filters");
                if (filtProp != null && filtProp.isArray)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Waifu Filters", EditorStyles.boldLabel);

                    var usedFilt = Enumerable.Range(0, filtProp.arraySize)
                                             .Select(i => (ProtectionFilterType)filtProp
                                                 .GetArrayElementAtIndex(i)
                                                 .FindPropertyRelative("filterType")
                                                 .enumValueIndex)
                                             .ToList();
                    int removeF = -1;
                    for (int i = 0; i < filtProp.arraySize; i++)
                    {
                        var entry = filtProp.GetArrayElementAtIndex(i);
                        var ftProp = entry.FindPropertyRelative("filterType");
                        var cur = (ProtectionFilterType)ftProp.enumValueIndex;

                        var allF = Enum.GetValues(typeof(ProtectionFilterType)).Cast<ProtectionFilterType>();
                        var choicesF = allF.Except(usedFilt.Where((_, idx) => idx != i)).ToArray();
                        var labelsF = choicesF.Select(c => c.ToString()).ToArray();
                        int idxF = Array.IndexOf(choicesF, cur);

                        EditorGUILayout.BeginHorizontal();
                        int selF = EditorGUILayout.Popup(idxF, labelsF, GUILayout.Width(100));
                        ftProp.enumValueIndex = (int)choicesF[selF];

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
                        filtProp.DeleteArrayElementAtIndex(removeF);

                    var availF = Enum.GetValues(typeof(ProtectionFilterType))
                                     .Cast<ProtectionFilterType>()
                                     .Except(usedFilt)
                                     .ToList();
                    if (availF.Count > 0 && GUILayout.Button("+ Add Filter"))
                    {
                        filtProp.InsertArrayElementAtIndex(filtProp.arraySize);
                        var ne = filtProp.GetArrayElementAtIndex(filtProp.arraySize - 1);
                        ne.FindPropertyRelative("filterType").enumValueIndex = (int)availF[0];
                        ne.FindPropertyRelative("summonCondition").enumValueIndex = 0;
                        ne.FindPropertyRelative("roleFilter").enumValueIndex = 0;
                        ne.FindPropertyRelative("elementFilter").enumValueIndex = 0;
                        ne.FindPropertyRelative("reignFilter").enumValueIndex = 0;
                    }
                }
            }
        }

        // 4) Duration
        EditorGUILayout.Space();
        var durProp = prop.FindPropertyRelative("duration");
        if (durProp != null)
        {
            EditorGUILayout.PropertyField(durProp, new GUIContent("Duration"));
            var dur = (Duration)durProp.enumValueIndex;
            if (dur == Duration.UntilTheNext)
            {
                var stageProp = prop.FindPropertyRelative("untilStage");
                var turnsProp = prop.FindPropertyRelative("durationTurns");
                if (stageProp != null) EditorGUILayout.PropertyField(stageProp, new GUIContent("Until Stage"));
                if (turnsProp != null)
                {
                    turnsProp.intValue = Mathf.Max(1, turnsProp.intValue);
                    EditorGUILayout.PropertyField(turnsProp, new GUIContent("Turns"));
                }
            }
            else if (dur == Duration.ForNumberTurns || dur == Duration.ForNumberOfYourTurns)
            {
                var turns2 = prop.FindPropertyRelative("durationTurns");
                if (turns2 != null)
                {
                    turns2.intValue = Mathf.Max(1, turns2.intValue);
                    EditorGUILayout.PropertyField(turns2, new GUIContent("Turns"));
                }
            }
        }
    }
}
#endif
