#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

[EffectDrawer(EffectType.SendWaifu, "sendWaifuEffect")]
public class SendWaifuEffectDrawer : IEffectDrawer
{
    public void Draw(SerializedProperty swProp)
    {
        if (swProp == null) return;

        // 1) Location
        var locProp = swProp.FindPropertyRelative("location");
        if (locProp != null)
            EditorGUILayout.PropertyField(locProp, new GUIContent("Location"));

        // 2) Selection Type (None/All/Select/Random/Situational)
        var selProp = swProp.FindPropertyRelative("selectionType");
        if (selProp != null)
        {
            EditorGUILayout.PropertyField(selProp, new GUIContent("Selection"));
            var sel = (Target)selProp.enumValueIndex;
            if (sel == Target.None)
                return;
        }

        // 3) Target Side
        var sideProp = swProp.FindPropertyRelative("targetSide");
        if (sideProp != null)
            EditorGUILayout.PropertyField(sideProp, new GUIContent("Target Side"));

        // 4) Amount (only for Select/Random/Situational)
        var amtProp = swProp.FindPropertyRelative("amount");
        if (amtProp != null && selProp != null)
        {
            var sel = (Target)selProp.enumValueIndex;
            if (sel == Target.Select || sel == Target.Random || sel == Target.Situational)
            {
                amtProp.intValue = Mathf.Max(1, amtProp.intValue);
                EditorGUILayout.PropertyField(amtProp, new GUIContent("Amount"));
            }
        }

        // 5) Situational extras
        if (selProp != null && (Target)selProp.enumValueIndex == Target.Situational)
        {
            var highLowProp = swProp.FindPropertyRelative("highLow");
            var statsProp = swProp.FindPropertyRelative("waifuStats");
            var tieBreakerProp = swProp.FindPropertyRelative("tieBreaker");

            if (highLowProp != null) EditorGUILayout.PropertyField(highLowProp, new GUIContent("Highest / Lowest"));
            if (statsProp != null) EditorGUILayout.PropertyField(statsProp, new GUIContent("Stat to Compare"));
            if (tieBreakerProp != null) EditorGUILayout.PropertyField(tieBreakerProp, new GUIContent("Tie Breaker"));
        }

        // 6) Dynamic Filters
        var filtersProp = swProp.FindPropertyRelative("filters");
        if (filtersProp == null || !filtersProp.isArray)
            return;

        EditorGUILayout.LabelField("Filters", EditorStyles.boldLabel);

        // Track which filter types are already used
        var used = Enumerable.Range(0, filtersProp.arraySize)
            .Select(i => {
                var entry = filtersProp.GetArrayElementAtIndex(i);
                var ft = entry?.FindPropertyRelative("filterType");
                return ft != null ? (SendFilterType?)ft.enumValueIndex : null;
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

            switch ((SendFilterType)ftProp.enumValueIndex)
            {
                case SendFilterType.SummonCondition:
                    var sc = entry.FindPropertyRelative("summonCondition");
                    if (sc != null) EditorGUILayout.PropertyField(sc, GUIContent.none);
                    break;
                case SendFilterType.Role:
                    var role = entry.FindPropertyRelative("roleFilter");
                    if (role != null) EditorGUILayout.PropertyField(role, GUIContent.none);
                    break;
                case SendFilterType.Element:
                    var elem = entry.FindPropertyRelative("elementFilter");
                    if (elem != null) EditorGUILayout.PropertyField(elem, GUIContent.none);
                    break;
            }

            if (GUILayout.Button("✕", GUILayout.Width(20)))
                removeIdx = i;
            EditorGUILayout.EndHorizontal();
        }

        if (removeIdx >= 0)
            filtersProp.DeleteArrayElementAtIndex(removeIdx);

        // “+ Add Filter” until all SendFilterType values are used
        var allTypes = System.Enum.GetValues(typeof(SendFilterType)).Cast<SendFilterType>();
        var available = allTypes.Except(used).ToList();
        if (available.Count > 0 && GUILayout.Button("+ Add Filter"))
        {
            filtersProp.InsertArrayElementAtIndex(filtersProp.arraySize);
            var newEntry = filtersProp.GetArrayElementAtIndex(filtersProp.arraySize - 1);

            var ftNew = newEntry.FindPropertyRelative("filterType");
            if (ftNew != null) ftNew.enumValueIndex = (int)available[0];

            var scNew = newEntry.FindPropertyRelative("summonCondition");
            if (scNew != null) scNew.enumValueIndex = 0;
            var roleNew = newEntry.FindPropertyRelative("roleFilter");
            if (roleNew != null) roleNew.enumValueIndex = 0;
            var elemNew = newEntry.FindPropertyRelative("elementFilter");
            if (elemNew != null) elemNew.enumValueIndex = 0;
        }
    }
}
#endif
