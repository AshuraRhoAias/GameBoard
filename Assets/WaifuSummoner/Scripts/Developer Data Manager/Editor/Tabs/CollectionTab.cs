// Assets/WaifuSummoner/Scripts/Developer Data Manager/Editor/Tabs/CollectionTab.cs
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public class CollectionTab
{
    // — UI state —
    private Vector2 scrollDecks, scrollCards, scrollDetails;
    private ReorderableList reorderableCardsList;

    // — Selection state —
    private string selectedDeck;
    private CollectionData selectedData;
    private List<string> cardPathList = new List<string>();
    private string selectedCardPath;
    private WaifuData selectedCard;

    // — Preview offsets (solo para waifus) —
    private readonly Vector2 lvlOffset = new Vector2(0, -4);
    private readonly Vector2 atkNumOff = new Vector2(-8, 36);
    private readonly Vector2 atkLabOff = new Vector2(-8, 38);
    private readonly Vector2 ambNumOff = new Vector2(50, 36);
    private readonly Vector2 ambLabOff = new Vector2(50, 38);

    // — Styling & drawer map —
    private Font carlitoFont;
    private GUIStyle style;
    private Dictionary<EffectType, (IEffectDrawer Drawer, string PropertyName)> _drawerMap;

    // — Deck list on disk —
    private string[] deckPaths = new string[0];

    public void Initialize()
    {
        RefreshDeckList();
        carlitoFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/WaifuSummoner/Fonts/Carlito-Bold.ttf");
        if (carlitoFont == null) Debug.LogError("No se encontró Assets/WaifuSummoner/Fonts/Carlito-Bold.ttf");
        style = new GUIStyle { font = carlitoFont, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        BuildDrawerMap();
    }

    void BuildDrawerMap()
    {
        _drawerMap = new Dictionary<EffectType, (IEffectDrawer, string)>();
        var asm = Assembly.GetAssembly(typeof(IEffectDrawer));
        foreach (var t in asm.GetTypes()
            .Where(x => typeof(IEffectDrawer).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract))
        {
            var attr = t.GetCustomAttribute<EffectDrawerAttribute>();
            if (attr != null)
                _drawerMap[attr.EffectType] = ((IEffectDrawer)System.Activator.CreateInstance(t), attr.PropertyName);
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
        foreach (var path in deckPaths)
            if (GUILayout.Button(Path.GetFileName(path)))
                SelectDeck(path);
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    void SelectDeck(string path)
    {
        selectedDeck = path;
        RefreshCardList();
        var dataPath = $"{selectedDeck}/CollectionData.asset";
        selectedData = AssetDatabase.LoadAssetAtPath<CollectionData>(dataPath);
        if (selectedData == null)
        {
            selectedData = ScriptableObject.CreateInstance<CollectionData>();
            selectedData.displayName = Path.GetFileName(selectedDeck);
            selectedData.identifier = selectedData.displayName.Length >= 3
                ? selectedData.displayName.Substring(0, 3).ToUpper()
                : selectedData.displayName.ToUpper();
            AssetDatabase.CreateAsset(selectedData, dataPath);
            AssetDatabase.SaveAssets();
        }
    }

    void DrawCardColumn()
    {
        GUILayout.BeginVertical(GUILayout.Width(250));
        if (selectedData != null)
        {
            GUILayout.Label($"Cards in {selectedData.displayName}", EditorStyles.boldLabel);
            if (GUILayout.Button("+ Add Card")) CreateNewCard();
            if (reorderableCardsList == null) SetupReorderableList();
            scrollCards = EditorGUILayout.BeginScrollView(scrollCards);
            reorderableCardsList.DoLayoutList();
            EditorGUILayout.EndScrollView();

            GUILayout.Space(8);
            EditorGUILayout.LabelField("Deck Info", EditorStyles.boldLabel);
            selectedData.format = (FormatType)EditorGUILayout.EnumPopup("Format", selectedData.format);

            EditorGUI.BeginChangeCheck();
            selectedData.displayName = EditorGUILayout.DelayedTextField("Name", selectedData.displayName);
            if (EditorGUI.EndChangeCheck())
            {
                var parent = Path.GetDirectoryName(selectedDeck);
                AssetDatabase.RenameAsset(selectedDeck, selectedData.displayName);
                AssetDatabase.SaveAssets();
                AssetDatabase.MoveAsset(
                    $"{parent}/CollectionData.asset",
                    $"{parent}/{selectedData.displayName}/CollectionData.asset"
                );
                AssetDatabase.SaveAssets();
                selectedDeck = $"{parent}/{selectedData.displayName}".Replace("\\", "/");
                RefreshDeckList();
                RefreshCardList();
            }

            EditorGUI.BeginChangeCheck();
            selectedData.identifier = EditorGUILayout.DelayedTextField("ID", selectedData.identifier);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(selectedData);
                AssetDatabase.SaveAssets();
                RenameAllCardsInDeck();
                RefreshCardList();
            }

            if (GUILayout.Button("Delete Card Group") &&
                EditorUtility.DisplayDialog("Confirm Delete", $"Delete '{selectedData.displayName}'?", "Yes", "No"))
            {
                AssetDatabase.DeleteAsset(selectedDeck);
                AssetDatabase.SaveAssets();
                selectedDeck = null;
                selectedData = null;
                RefreshDeckList();
                RefreshCardList();
            }
        }
        GUILayout.EndVertical();
    }

    void SetupReorderableList()
    {
        cardPathList = Directory.Exists(selectedDeck)
            ? Directory.GetFiles(selectedDeck, "*.asset").Where(f => !f.EndsWith("CollectionData.asset")).ToList()
            : new List<string>();

        reorderableCardsList = new ReorderableList(cardPathList, typeof(string), true, false, false, false);
        reorderableCardsList.drawHeaderCallback = _ => { };
        reorderableCardsList.drawElementCallback = (rect, i, _, _) =>
        {
            var path = cardPathList[i];
            var name = Path.GetFileNameWithoutExtension(path);
            if (GUI.Button(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), name))
            {
                selectedCardPath = path;
                LoadCard();
            }
        };
        reorderableCardsList.onReorderCallback = _ =>
        {
            RenameAllCardsInDeck();
            RefreshCardList();
        };
    }

    void RefreshCardList()
    {
        cardPathList = !string.IsNullOrEmpty(selectedDeck)
            ? Directory.GetFiles(selectedDeck, "*.asset").Where(f => !f.EndsWith("CollectionData.asset")).ToList()
            : new List<string>();
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
            var newName = $"{selectedData.identifier}-{idx} {safeName}.asset";
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

            // — Card Type sin 'Any', default Waifu —
            var cardTypeProp = so.FindProperty("cardType");
            var current = (CardType)cardTypeProp.enumValueIndex;
            var entries = System.Enum.GetValues(typeof(CardType))
                             .Cast<CardType>()
                             .ToArray(); // Eliminado el filtro por 'Any'
            int sel = System.Array.IndexOf(entries, current);
            if (sel < 0) sel = System.Array.IndexOf(entries, CardType.Waifu);
            sel = EditorGUILayout.Popup("Card Type", sel, entries.Select(e => e.ToString()).ToArray());
            cardTypeProp.enumValueIndex = (int)entries[sel];

            // — Si es WAIFU, dibujar todos estos campos —
            if ((CardType)cardTypeProp.enumValueIndex == CardType.Waifu)
            {
                EditorGUILayout.PropertyField(so.FindProperty("rarity"), new GUIContent("Rarity"));
                EditorGUILayout.PropertyField(so.FindProperty("waifuName"), new GUIContent("Waifu Name"));
                EditorGUILayout.PropertyField(so.FindProperty("reign"), new GUIContent("Reign"));
                EditorGUILayout.PropertyField(so.FindProperty("classType"), new GUIContent("Class Type"));
                EditorGUILayout.PropertyField(so.FindProperty("atk"), new GUIContent("Attack"));
                EditorGUILayout.PropertyField(so.FindProperty("ambush"), new GUIContent("Ambush"));
            }

            so.ApplyModifiedProperties();
        }

        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    void CreateNewDeck()
    {
        var path = EditorUtility.SaveFolderPanel("Create New Deck", "Assets/WaifuSummoner/Decks", "");
        if (!string.IsNullOrEmpty(path))
        {
            var deck = ScriptableObject.CreateInstance<CollectionData>(); // Corregido
            deck.displayName = Path.GetFileName(path);
            deck.identifier = "NEW";
            AssetDatabase.CreateAsset(deck, $"{path}/CollectionData.asset");
            AssetDatabase.SaveAssets();
            RefreshDeckList();
        }
    }

    void CreateNewCard()
    {
        if (string.IsNullOrEmpty(selectedDeck)) return;

        var card = ScriptableObject.CreateInstance<WaifuData>(); // Corregido
        var cardName = "New Card";
        var path = $"{selectedDeck}/{cardName}.asset";
        AssetDatabase.CreateAsset(card, path);
        AssetDatabase.SaveAssets();
        RefreshCardList();
    }

    void LoadCard()
    {
        if (string.IsNullOrEmpty(selectedCardPath)) return;

        selectedCard = AssetDatabase.LoadAssetAtPath<WaifuData>(selectedCardPath);
    }

    void DrawPreviewColumn()
    {
        GUILayout.BeginVertical(GUILayout.Width(250));
        GUILayout.Label("Preview", EditorStyles.boldLabel);
        if (selectedCard != null)
        {
            // Preview code based on `selectedCard` details
        }
        GUILayout.EndVertical();
    }

    void RefreshDeckList()
    {
        deckPaths = Directory.GetDirectories("Assets/WaifuSummoner/Decks")
                              .Where(d => Directory.Exists(d) && File.Exists($"{d}/CollectionData.asset"))
                              .ToArray();
    }
}
#endif