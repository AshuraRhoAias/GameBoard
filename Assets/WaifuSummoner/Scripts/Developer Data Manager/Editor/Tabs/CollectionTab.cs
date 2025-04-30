// Assets/Scripts/Editor/Tabs/CollectionTab.cs
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public enum FormatType { Collection, Deck }

public class CollectionTab
{
    // — UI state —
    private Vector2 scrollDecks, scrollCards, scrollDetails;
    private ReorderableList reorderableCardsList;

    // — Selection state —
    private string selectedDeck;
    private List<string> cardPathList = new List<string>();
    private string selectedCardPath;
    private WaifuData selectedCard;

    // — Deck metadata editable —
    private FormatType format = FormatType.Deck;
    private string deckDisplayName = "";
    private string deckIdentifier = "";

    // — Preview offsets —
    private readonly Vector2 lvlOffset = new Vector2(0, -4);
    private readonly Vector2 atkNumOff = new Vector2(-8, 36);
    private readonly Vector2 atkLabOff = new Vector2(-8, 38);
    private readonly Vector2 ambNumOff = new Vector2(50, 36);
    private readonly Vector2 ambLabOff = new Vector2(50, 38);

    // — Styling —
    private Font carlitoFont;
    private GUIStyle style;

    // — Deck list on disk —
    private string[] deckPaths = new string[0];

    // — Drawer map: EffectType → (drawer, propertyName) —
    private Dictionary<EffectType, (IEffectDrawer Drawer, string PropertyName)> _drawerMap;

    public void Initialize()
    {
        RefreshDeckList();
        carlitoFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/WaifuSummoner/Fonts/Carlito-Bold.ttf");
        if (carlitoFont == null) Debug.LogError("No se encontró Assets/WaifuSummoner/Fonts/Carlito-Bold.ttf");
        style = new GUIStyle
        {
            font = carlitoFont,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };

        BuildDrawerMap();
    }

    void BuildDrawerMap()
    {
        _drawerMap = new Dictionary<EffectType, (IEffectDrawer, string)>();
        var asm = Assembly.GetAssembly(typeof(IEffectDrawer));
        foreach (var t in asm.GetTypes()
            .Where(x => typeof(IEffectDrawer).IsAssignableFrom(x)
                     && !x.IsInterface && !x.IsAbstract))
        {
            var attr = t.GetCustomAttribute<EffectDrawerAttribute>();
            if (attr != null)
            {
                var inst = (IEffectDrawer)System.Activator.CreateInstance(t);
                _drawerMap[attr.EffectType] = (inst, attr.PropertyName);
            }
        }
    }

    public void DrawTab()
    {
        EditorGUILayout.BeginHorizontal();
        DrawDeckColumn();
        DrawCardColumn();
        DrawDetailsColumn();
        DrawPreviewColumn();
        EditorGUILayout.EndHorizontal();
    }

    void DrawDeckColumn()
    {
        GUILayout.BeginVertical(GUILayout.Width(200));
        GUILayout.Label("Decks / Sets", EditorStyles.boldLabel);
        if (GUILayout.Button("+ Add Set")) CreateNewDeck();

        scrollDecks = EditorGUILayout.BeginScrollView(scrollDecks);
        foreach (var p in deckPaths)
        {
            if (GUILayout.Button(Path.GetFileName(p)))
            {
                selectedDeck = p;
                RefreshCardList();
                // inicializar campos
                deckDisplayName = Path.GetFileName(selectedDeck);
                deckIdentifier = deckDisplayName.Length >= 3
                    ? deckDisplayName.Substring(0, 3).ToUpper()
                    : deckDisplayName.ToUpper();
                format = FormatType.Deck;
            }
        }
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    void DrawCardColumn()
    {
        GUILayout.BeginVertical(GUILayout.Width(250));
        GUILayout.Label($"Cards in {deckDisplayName}", EditorStyles.boldLabel);
        if (!string.IsNullOrEmpty(selectedDeck) && GUILayout.Button("+ Add Card"))
            CreateNewCard();

        if (reorderableCardsList == null)
            SetupReorderableList();

        scrollCards = EditorGUILayout.BeginScrollView(scrollCards);
        reorderableCardsList.DoLayoutList();
        EditorGUILayout.EndScrollView();

        // ── NUEVO UI: Metadatos del deck ──
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Deck Info", EditorStyles.boldLabel);
        format = (FormatType)EditorGUILayout.EnumPopup("Format", format);
        deckDisplayName = EditorGUILayout.TextField("Name", deckDisplayName);
        deckIdentifier = EditorGUILayout.TextField("ID", deckIdentifier);

        if (GUILayout.Button("Delete Card Group"))
        {
            if (EditorUtility.DisplayDialog(
                "Confirm Delete",
                $"Delete entire group '{deckDisplayName}'?",
                "Yes", "No"))
            {
                AssetDatabase.DeleteAsset(selectedDeck);
                AssetDatabase.Refresh();
                selectedDeck = null;
                RefreshDeckList();
                RefreshCardList();
            }
        }
        // ────────────────────────────────────

        GUILayout.EndVertical();
    }

    void SetupReorderableList()
    {
        cardPathList = new List<string>(
            Directory.Exists(selectedDeck)
                ? Directory.GetFiles(selectedDeck, "*.asset")
                : new string[0]
        );

        reorderableCardsList = new ReorderableList(cardPathList, typeof(string), true, false, false, false);
        reorderableCardsList.drawHeaderCallback = rect => { };
        reorderableCardsList.drawElementCallback = (rect, index, _, _) =>
        {
            var path = cardPathList[index];
            var name = Path.GetFileNameWithoutExtension(path);
            if (GUI.Button(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), name))
            {
                selectedCardPath = path;
                LoadCard();
            }
        };
        reorderableCardsList.onReorderCallback = _ => {
            RenameAllCardsInDeck();
            RefreshCardList();
        };
    }

    void RefreshCardList()
    {
        cardPathList = new List<string>(
            !string.IsNullOrEmpty(selectedDeck)
                ? Directory.GetFiles(selectedDeck, "*.asset")
                : new string[0]
        );
        reorderableCardsList = null;
    }

    void RenameAllCardsInDeck()
    {
        for (int i = 0; i < cardPathList.Count; i++)
        {
            var oldPath = cardPathList[i];
            var card = AssetDatabase.LoadAssetAtPath<WaifuData>(oldPath);
            if (card == null) continue;

            var idx = (i + 1).ToString("000");
            var safeName = card.waifuName.Replace('/', '_').Replace('\\', '_');
            var newName = $"{deckIdentifier}-{idx} {safeName}.asset";
            if (Path.GetFileName(oldPath) != newName)
                AssetDatabase.RenameAsset(oldPath, newName);
        }
        AssetDatabase.SaveAssets();
    }

    void DrawDetailsColumn()
    {
        GUILayout.BeginVertical(GUILayout.Width(350));
        GUILayout.Label("Card Details", EditorStyles.boldLabel);
        scrollDetails = EditorGUILayout.BeginScrollView(scrollDetails);

        if (selectedCard != null)
        {
            var so = new SerializedObject(selectedCard);
            so.Update();

            EditorGUILayout.PropertyField(so.FindProperty("cardType"), new GUIContent("Card Type"));
            EditorGUILayout.PropertyField(so.FindProperty("rarity"), new GUIContent("Rarity"));
            EditorGUILayout.PropertyField(so.FindProperty("waifuName"), new GUIContent("Waifu Name"));
            EditorGUILayout.PropertyField(so.FindProperty("reign"), new GUIContent("Reign"));
            EditorGUILayout.PropertyField(so.FindProperty("role"));
            EditorGUILayout.PropertyField(so.FindProperty("element"));
            EditorGUILayout.PropertyField(so.FindProperty("level"));
            EditorGUILayout.PropertyField(so.FindProperty("summonType"), new GUIContent("Summon Type"));

            DrawStatField(so, "attack", "Attack");
            DrawStatField(so, "ambush", "Ambush");

            var effectsProp = so.FindProperty("effects");
            EditorGUILayout.LabelField("Effects", EditorStyles.boldLabel);
            int removeIdx = -1;

            for (int i = 0; i < effectsProp.arraySize; i++)
            {
                var eProp = effectsProp.GetArrayElementAtIndex(i);
                var triggers = eProp.FindPropertyRelative("triggers");
                var typeProp = eProp.FindPropertyRelative("effectType");

                if (triggers.arraySize == 0)
                {
                    triggers.InsertArrayElementAtIndex(0);
                    triggers.GetArrayElementAtIndex(0).enumValueIndex = (int)Trigger.Action;
                }

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Triggers", EditorStyles.boldLabel);

                int removeTriggerIdx = -1;
                for (int j = 0; j < triggers.arraySize; j++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(
                        triggers.GetArrayElementAtIndex(j),
                        new GUIContent($"Trigger {j + 1}")
                    );
                    if (GUILayout.Button("–", GUILayout.Width(20)))
                        removeTriggerIdx = j;
                    EditorGUILayout.EndHorizontal();
                }
                if (removeTriggerIdx >= 0)
                    triggers.DeleteArrayElementAtIndex(removeTriggerIdx);

                if (GUILayout.Button("+ Add Trigger"))
                    triggers.InsertArrayElementAtIndex(triggers.arraySize);

                EditorGUILayout.PropertyField(typeProp, new GUIContent("Effect Type"));
                var eType = (EffectType)typeProp.enumValueIndex;

                if (_drawerMap.TryGetValue(eType, out var entry))
                {
                    EditorGUILayout.LabelField($"{eType} Effect", EditorStyles.boldLabel);
                    entry.Drawer.Draw(eProp.FindPropertyRelative(entry.PropertyName));
                }
                else
                {
                    EditorGUILayout.HelpBox("No drawer for " + eType, MessageType.Info);
                }

                EditorGUILayout.PropertyField(
                    eProp.FindPropertyRelative("effectDescription"),
                    new GUIContent("Description")
                );
                if (GUILayout.Button("Remove Effect"))
                    removeIdx = i;

                EditorGUILayout.EndVertical();
                GUILayout.Space(4);
            }

            if (removeIdx >= 0)
                effectsProp.DeleteArrayElementAtIndex(removeIdx);

            if (GUILayout.Button("+ Add Effect"))
            {
                int idxNew = effectsProp.arraySize;
                effectsProp.InsertArrayElementAtIndex(idxNew);
                var newE = effectsProp.GetArrayElementAtIndex(idxNew);
                newE.FindPropertyRelative("effectType").enumValueIndex = (int)EffectType.None;
                var newT = newE.FindPropertyRelative("triggers");
                newT.ClearArray();
                newT.InsertArrayElementAtIndex(0);
                newT.GetArrayElementAtIndex(0).enumValueIndex = (int)Trigger.Action;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(so.FindProperty("artwork"), new GUIContent("Full Card Image"));
            if (GUILayout.Button("Browse…", GUILayout.Width(70))) SelectArtwork();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(8);
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_TreeEditor.Trash"), GUILayout.Height(24)))
            {
                if (EditorUtility.DisplayDialog("Confirm Delete", $"Delete '{selectedCard.waifuName}'?", "Yes", "No"))
                {
                    AssetDatabase.DeleteAsset(selectedCardPath);
                    selectedCard = null;
                    selectedCardPath = null;
                    RefreshCardList();
                    GUIUtility.ExitGUI();
                }
            }

            so.ApplyModifiedProperties();
            RenameAssetToMatchName();
        }
        else
        {
            GUILayout.Label("Select a card.");
        }

        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    void DrawStatField(SerializedObject so, string propName, string label)
    {
        var prop = so.FindProperty(propName);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(label);
        prop.intValue = Mathf.RoundToInt(EditorGUILayout.Slider(prop.intValue, 0, 30));
        prop.intValue = EditorGUILayout.IntField(prop.intValue, GUILayout.Width(50));
        prop.intValue = Mathf.Clamp(prop.intValue, 0, 999);
        EditorGUILayout.EndHorizontal();
    }

    void RenameAssetToMatchName()
    {
        var oldName = Path.GetFileNameWithoutExtension(selectedCardPath);
        var prefix = oldName.Split(' ')[0];
        var safe = selectedCard.waifuName.Replace('/', '_').Replace('\\', '_');
        var newName = $"{prefix} {safe}.asset";
        if (Path.GetFileName(selectedCardPath) != newName)
        {
            AssetDatabase.RenameAsset(selectedCardPath, newName);
            AssetDatabase.SaveAssets();
            var dir = Path.GetDirectoryName(selectedCardPath);
            selectedCardPath = Path.Combine(dir ?? "", newName).Replace('\\', '/');
            RefreshCardList();
        }
    }

    void DrawPreviewColumn()
    {
        const float previewW = 320f;
        GUILayout.BeginVertical(GUILayout.Width(previewW));
        GUILayout.Label("Preview", EditorStyles.boldLabel);

        if (selectedCard == null || selectedCard.artwork == null)
        {
            GUILayout.Label("No Preview");
        }
        else
        {
            var tex = selectedCard.artwork.texture;
            float aspect = (float)tex.width / tex.height;
            Rect rect = GUILayoutUtility.GetAspectRect(aspect, GUILayout.Width(previewW));
            GUI.DrawTexture(rect, tex, ScaleMode.ScaleToFit, true);

            int numFS = Mathf.RoundToInt(rect.height * 0.06f);
            int lblFS = Mathf.RoundToInt(rect.height * 0.03f);
            Color32 fill = new Color32(0xFC, 0xF4, 0xB6, 255);

            void DrawText(string txt, Rect r, int fs)
            {
                style.fontSize = fs;
                style.normal.textColor = new Color32(0x84, 0x27, 0x48, 255);
                for (int dx = -2; dx <= 2; dx++)
                    for (int dy = -2; dy <= 2; dy++)
                        if (dx != 0 || dy != 0)
                            GUI.Label(new Rect(r.x + dx, r.y + dy, r.width, r.height), txt, style);
                style.normal.textColor = fill;
                GUI.Label(r, txt, style);
            }

            DrawText(
                selectedCard.level.ToString(),
                new Rect(new Vector2(rect.x + rect.width * 0.06f, rect.y + rect.height * 0.01f) + lvlOffset,
                         new Vector2(rect.width * 0.12f, rect.height * 0.12f)),
                numFS
            );
            DrawText(
                selectedCard.attack.ToString(),
                new Rect(new Vector2(rect.x + rect.width * 0.06f, rect.y + rect.height * 0.82f) + atkNumOff,
                         new Vector2(rect.width * 0.12f, rect.height * 0.12f)),
                numFS
            );
            DrawText(
                "Atk",
                new Rect(new Vector2(rect.x + rect.width * 0.06f, rect.y + rect.height * 0.82f) + atkLabOff,
                         new Vector2(rect.width * 0.12f, rect.height * 0.12f * 0.4f)),
                lblFS
            );
            DrawText(
                selectedCard.ambush.ToString(),
                new Rect(new Vector2(rect.x + rect.width * 0.80f - rect.width * 0.12f, rect.y + rect.height * 0.82f) + ambNumOff,
                         new Vector2(rect.width * 0.12f, rect.height * 0.12f)),
                numFS
            );
            DrawText(
                "Amb",
                new Rect(new Vector2(rect.x + rect.width * 0.80f - rect.width * 0.12f, rect.y + rect.height * 0.82f) + ambLabOff,
                         new Vector2(rect.width * 0.12f, rect.height * 0.12f * 0.4f)),
                lblFS
            );
        }

        GUILayout.EndVertical();
    }

    // — Disk helpers —
    void RefreshDeckList()
    {
        const string basePath = "Assets/WaifuSummoner/Cards";
        if (!Directory.Exists(basePath))
            Directory.CreateDirectory(basePath);
        deckPaths = Directory.GetDirectories(basePath);
    }

    void LoadCard()
    {
        selectedCard = AssetDatabase.LoadAssetAtPath<WaifuData>(selectedCardPath);
    }

    void CreateNewDeck()
    {
        var p = AssetDatabase.GenerateUniqueAssetPath("Assets/WaifuSummoner/Cards/NewDeck");
        Directory.CreateDirectory(p);
        AssetDatabase.Refresh();
        RefreshDeckList();
    }

    void CreateNewCard()
    {
        var card = ScriptableObject.CreateInstance<WaifuData>();
        card.waifuName = "New Waifu";
        // usa deckIdentifier como prefijo
        int idx = cardPathList.Count + 1;
        string is0 = idx.ToString("000");
        string fn = $"{deckIdentifier}-{is0} {card.waifuName}.asset";
        string ap = AssetDatabase.GenerateUniqueAssetPath($"{selectedDeck}/{fn}");
        AssetDatabase.CreateAsset(card, ap);
        AssetDatabase.SaveAssets();
        RefreshCardList();
    }

    void SelectArtwork()
    {
        if (selectedCard == null) return;
        var p = EditorUtility.OpenFilePanel("Select Full Card Image", "", "png,jpg,jpeg");
        if (string.IsNullOrEmpty(p)) return;

        string dn = Path.GetFileName(selectedDeck);
        string td = $"Assets/WaifuSummoner/Cards/Artwork/{dn}";
        if (!Directory.Exists(td)) Directory.CreateDirectory(td);

        string fn = Path.GetFileName(p);
        string tp = Path.Combine(td, fn);
        File.Copy(p, tp, true);
        AssetDatabase.Refresh();

        if (AssetImporter.GetAtPath(tp) is TextureImporter imp)
        {
            imp.textureType = TextureImporterType.Sprite;
            imp.spriteImportMode = SpriteImportMode.Single;
            imp.SaveAndReimport();
        }

        selectedCard.artwork = AssetDatabase.LoadAssetAtPath<Sprite>(tp);
        EditorUtility.SetDirty(selectedCard);
        AssetDatabase.SaveAssets();
    }
}
#endif
