// Assets/WaifuSummoner/Scripts/Developer Data Manager/Editor/Drawers/SummonAidEffectDrawer.cs
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[EffectDrawer(EffectType.SummonAid, "summonAidEffect")]
public class SummonAidEffectDrawer : IEffectDrawer
{
    public void Draw(SerializedProperty mProp)
    {
        if (mProp == null) return;

        // 1) SummonKind
        var kindProp = mProp.FindPropertyRelative("summonKind");
        EditorGUILayout.PropertyField(kindProp, new GUIContent("Summon Kind"));
        var kind = (SummonKind)kindProp.enumValueIndex;

        EditorGUI.indentLevel++;
        switch (kind)
        {
            case SummonKind.TurnSummon:
                DrawTurnSummon(mProp);
                break;
            case SummonKind.SeductionSummon:
                DrawSeductionSummon(mProp);
                break;
            default:
                // None → nothing
                break;
        }
        EditorGUI.indentLevel--;
    }

    private void DrawTurnSummon(SerializedProperty mProp)
    {
        // Extra Summons
        var extraProp = mProp.FindPropertyRelative("extraTurnSummons");
        EditorGUILayout.PropertyField(extraProp, new GUIContent("Extra Summons"));
        if ((ExtraOption)extraProp.enumValueIndex == ExtraOption.Custom)
        {
            var customExtra = mProp.FindPropertyRelative("customExtraTurnSummons");
            customExtra.intValue = Mathf.Max(1, customExtra.intValue);
            EditorGUILayout.PropertyField(customExtra, new GUIContent("  Custom Count"));
        }

        // Reduce Level
        var reduceProp = mProp.FindPropertyRelative("reduceLevelBy");
        reduceProp.intValue = Mathf.Max(0, reduceProp.intValue);
        EditorGUILayout.PropertyField(reduceProp, new GUIContent("Reduce Level By"));

        // Number of Summons
        var numSumProp = mProp.FindPropertyRelative("numberOfSummons");
        numSumProp.intValue = Mathf.Max(1, numSumProp.intValue);
        EditorGUILayout.PropertyField(numSumProp, new GUIContent("Number of Summons"));
    }

    private void DrawSeductionSummon(SerializedProperty mProp)
    {
        // Extra Seduction Summons
        var extraProp = mProp.FindPropertyRelative("extraSeductionSummons");
        EditorGUILayout.PropertyField(extraProp, new GUIContent("Extra Summons"));
        if ((ExtraOption)extraProp.enumValueIndex == ExtraOption.Custom)
        {
            var customExtra = mProp.FindPropertyRelative("customExtraSeductionSummons");
            customExtra.intValue = Mathf.Max(1, customExtra.intValue);
            EditorGUILayout.PropertyField(customExtra, new GUIContent("  Custom Count"));
        }

        // Reduce Waiting Turns
        var waitProp = mProp.FindPropertyRelative("reduceWaitingTurns");
        waitProp.intValue = Mathf.Max(0, waitProp.intValue);
        EditorGUILayout.PropertyField(waitProp, new GUIContent("Reduce Waiting Turns"));

        // Remove After
        var remProp = mProp.FindPropertyRelative("removeAfter");
        EditorGUILayout.PropertyField(remProp, new GUIContent("Remove Effect After"));
        var rem = (RemoveAfter)remProp.enumValueIndex;
        switch (rem)
        {
            case RemoveAfter.CustomNumberOfSummons:
                var cntProp = mProp.FindPropertyRelative("customRemoveCount");
                cntProp.intValue = Mathf.Max(1, cntProp.intValue);
                EditorGUILayout.PropertyField(cntProp, new GUIContent("  Summon Count"));
                break;
            case RemoveAfter.CustomNumberOfTurns:
                var turnProp = mProp.FindPropertyRelative("customRemoveTurns");
                turnProp.intValue = Mathf.Max(1, turnProp.intValue);
                EditorGUILayout.PropertyField(turnProp, new GUIContent("  Turn Count"));
                break;
            case RemoveAfter.Permanent:
            default:
                // nothing more
                break;
        }
    }
}
#endif
