// Assets/WaifuSummoner/Scripts/Developer Data Manager/Editor/Tabs/CollectionEffectDrawers/RecycleEffectDrawer.cs
#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[EffectDrawer(EffectType.Recycle, "recycleEffect")]
public class RecycleEffectDrawer : IEffectDrawer
{
    public void Draw(SerializedProperty prop)
    {
        if (prop == null) return;

        // ... (puntos 1–3 idénticos) ...

        // 4) RECYCLE FILTERS (sobre lo reciclado)
        var filtProp = prop.FindPropertyRelative("recycleFilters");
        if (filtProp != null && filtProp.isArray)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Recycle Filters", EditorStyles.boldLabel);

            // recopilar usados
            int remF = -1;
            var usedTypes = new System.Collections.Generic.List<RecycleEffectData.RecycleFilterType>();
            for (int i = 0; i < filtProp.arraySize; i++)
            {
                var entry = filtProp.GetArrayElementAtIndex(i);
                var ft = entry.FindPropertyRelative("filterType");
                usedTypes.Add((RecycleEffectData.RecycleFilterType)ft.enumValueIndex);
            }

            // dibujar cada filtro
            for (int i = 0; i < filtProp.arraySize; i++)
            {
                var entry = filtProp.GetArrayElementAtIndex(i);
                var ftProp = entry.FindPropertyRelative("filterType");
                var current = (RecycleEffectData.RecycleFilterType)ftProp.enumValueIndex;

                // popup de tipos disponibles (excluyendo los ya usados salvo este)
                var choices = Enum.GetValues(typeof(RecycleEffectData.RecycleFilterType))
                                  .Cast<RecycleEffectData.RecycleFilterType>()
                                  .Except(usedTypes.Where((_, idx) => idx != i))
                                  .ToArray();
                int idx = Array.IndexOf(choices, current);
                idx = Mathf.Max(0, idx);
                EditorGUILayout.BeginHorizontal();
                idx = EditorGUILayout.Popup(idx, choices.Select(c => c.ToString()).ToArray(), GUILayout.Width(100));
                ftProp.enumValueIndex = (int)choices[idx];

                // luego el campo concreto según tipo
                switch ((RecycleEffectData.RecycleFilterType)ftProp.enumValueIndex)
                {
                    case RecycleEffectData.RecycleFilterType.CardType:
                        EditorGUILayout.PropertyField(entry.FindPropertyRelative("cardType"), GUIContent.none);
                        break;
                    case RecycleEffectData.RecycleFilterType.Reign:
                        EditorGUILayout.PropertyField(entry.FindPropertyRelative("reignFilter"), GUIContent.none);
                        break;
                    case RecycleEffectData.RecycleFilterType.Role:
                        EditorGUILayout.PropertyField(entry.FindPropertyRelative("roleFilter"), GUIContent.none);
                        break;
                    case RecycleEffectData.RecycleFilterType.Element:
                        EditorGUILayout.PropertyField(entry.FindPropertyRelative("elementFilter"), GUIContent.none);
                        break;
                }

                if (GUILayout.Button("✕", GUILayout.Width(20)))
                    remF = i;
                EditorGUILayout.EndHorizontal();
            }

            if (remF >= 0)
                filtProp.DeleteArrayElementAtIndex(remF);

            // botón para añadir un nuevo filtro
            var avail = Enum.GetValues(typeof(RecycleEffectData.RecycleFilterType))
                            .Cast<RecycleEffectData.RecycleFilterType>()
                            .Except(usedTypes)
                            .ToList();
            if (avail.Count > 0 && GUILayout.Button("+ Add Recycle Filter"))
            {
                filtProp.InsertArrayElementAtIndex(filtProp.arraySize);
                var newEntry = filtProp.GetArrayElementAtIndex(filtProp.arraySize - 1);
                newEntry.FindPropertyRelative("filterType").enumValueIndex = (int)avail[0];
                // inicializar sus subpropiedades a 0
                newEntry.FindPropertyRelative("cardType").enumValueIndex = 0;
                newEntry.FindPropertyRelative("reignFilter").enumValueIndex = 0;
                newEntry.FindPropertyRelative("roleFilter").enumValueIndex = 0;
                newEntry.FindPropertyRelative("elementFilter").enumValueIndex = 0;
            }
        }

        EditorGUILayout.Space();

        // ... (puntos 5–7 idénticos) ...

        // 8) DRAW FILTERS (sobre lo robado)
        var dFilt = prop.FindPropertyRelative("drawFilters");
        if (dFilt != null && dFilt.isArray)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Draw Filters", EditorStyles.boldLabel);

            int remD = -1;
            var usedDTypes = new System.Collections.Generic.List<RecycleEffectData.RecycleFilterType>();
            for (int i = 0; i < dFilt.arraySize; i++)
            {
                var entry = dFilt.GetArrayElementAtIndex(i);
                var ft = entry.FindPropertyRelative("filterType");
                usedDTypes.Add((RecycleEffectData.RecycleFilterType)ft.enumValueIndex);
            }

            for (int i = 0; i < dFilt.arraySize; i++)
            {
                var entry = dFilt.GetArrayElementAtIndex(i);
                var ftProp = entry.FindPropertyRelative("filterType");
                var current = (RecycleEffectData.RecycleFilterType)ftProp.enumValueIndex;

                var choices = Enum.GetValues(typeof(RecycleEffectData.RecycleFilterType))
                                  .Cast<RecycleEffectData.RecycleFilterType>()
                                  .Except(usedDTypes.Where((_, idx) => idx != i))
                                  .ToArray();
                int idx = Array.IndexOf(choices, current);
                idx = Mathf.Max(0, idx);
                EditorGUILayout.BeginHorizontal();
                idx = EditorGUILayout.Popup(idx, choices.Select(c => c.ToString()).ToArray(), GUILayout.Width(100));
                ftProp.enumValueIndex = (int)choices[idx];

                switch ((RecycleEffectData.RecycleFilterType)ftProp.enumValueIndex)
                {
                    case RecycleEffectData.RecycleFilterType.CardType:
                        EditorGUILayout.PropertyField(entry.FindPropertyRelative("cardType"), GUIContent.none);
                        break;
                    case RecycleEffectData.RecycleFilterType.Reign:
                        EditorGUILayout.PropertyField(entry.FindPropertyRelative("reignFilter"), GUIContent.none);
                        break;
                    case RecycleEffectData.RecycleFilterType.Role:
                        EditorGUILayout.PropertyField(entry.FindPropertyRelative("roleFilter"), GUIContent.none);
                        break;
                    case RecycleEffectData.RecycleFilterType.Element:
                        EditorGUILayout.PropertyField(entry.FindPropertyRelative("elementFilter"), GUIContent.none);
                        break;
                }

                if (GUILayout.Button("✕", GUILayout.Width(20)))
                    remD = i;
                EditorGUILayout.EndHorizontal();
            }

            if (remD >= 0)
                dFilt.DeleteArrayElementAtIndex(remD);

            var availD = Enum.GetValues(typeof(RecycleEffectData.RecycleFilterType))
                             .Cast<RecycleEffectData.RecycleFilterType>()
                             .Except(usedDTypes)
                             .ToList();
            if (availD.Count > 0 && GUILayout.Button("+ Add Draw Filter"))
            {
                dFilt.InsertArrayElementAtIndex(dFilt.arraySize);
                var newEntry = dFilt.GetArrayElementAtIndex(dFilt.arraySize - 1);
                newEntry.FindPropertyRelative("filterType").enumValueIndex = (int)availD[0];
                newEntry.FindPropertyRelative("cardType").enumValueIndex = 0;
                newEntry.FindPropertyRelative("reignFilter").enumValueIndex = 0;
                newEntry.FindPropertyRelative("roleFilter").enumValueIndex = 0;
                newEntry.FindPropertyRelative("elementFilter").enumValueIndex = 0;
            }
        }
    }
}
#endif
