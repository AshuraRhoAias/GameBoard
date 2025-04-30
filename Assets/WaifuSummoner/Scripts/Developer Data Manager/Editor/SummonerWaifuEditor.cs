#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class SummonerWaifuEditor : EditorWindow
{
    private int currentTab = 0;
    private string[] tabs = { "Collection", "Types & Attributes" }; // agrega más tabs cuando lo necesites

    private CollectionTab collectionTab;

    [MenuItem("Tools/Summoner Waifu Editor")]
    public static void ShowWindow()
    {
        GetWindow<SummonerWaifuEditor>("Summoner Waifu Editor");
    }

    private void OnEnable()
    {
        collectionTab = new CollectionTab();
        collectionTab.Initialize();
    }

    private void OnGUI()
    {
        currentTab = GUILayout.Toolbar(currentTab, tabs);

        switch (currentTab)
        {
            case 0:
                collectionTab.DrawTab();
                break;

            case 1:
                GUILayout.Label("Types & Attributes (próximamente)");
                break;
        }
    }
}
#endif
