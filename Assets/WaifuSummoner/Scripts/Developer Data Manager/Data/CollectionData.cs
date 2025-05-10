using UnityEngine;

public enum FormatType { Collection, Deck }

[CreateAssetMenu(menuName = "WaifuSummoner/Collection Data", fileName = "NewCollectionData")]
public class CollectionData : ScriptableObject
{
    public FormatType format = FormatType.Deck;
    public string displayName = "NewDeck";
    public string identifier = "NEW";
}
