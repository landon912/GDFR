using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class GameSettings : NetworkBehaviour
{
    public enum Difficulty { Easy = 0, Medium = 1, Hard = 2, VeryHard = 3 }
    public enum CardVariant { Rhymes = 0, Numbers = 1 }
    public enum RulesVariant { Classic = 0, Solitaire = 1, UltimateSolitaire = 2, GoblinsRule = 4 }

    [SyncVar(hook="UpdatePlayerNumber")]
    public int numberOfPlayers;
    [SyncVar(hook="UpdateDifficultyLevel")]
    public Difficulty difficultyLevel;
    [SyncVar(hook="UpdateCardVariant")]
    public CardVariant cardVariant;
    [SyncVar(hook="UpdateRulesVariant")]
    public RulesVariant rulesVariant;

    public delegate void NumberOfPlayersChangedHandler(int count);
    public static event NumberOfPlayersChangedHandler NumberOfPlayersChangedEvent;

    public delegate void DifficultyLevelChangedHandler(int difficulty);
    public static event DifficultyLevelChangedHandler DifficultyLevelChangedEvent;

    public delegate void CardVariantChangedHandler(int cardVariant);
    public static event CardVariantChangedHandler CardVariantChangedEvent;

    public delegate void RulesVariantChangedHandler(int rulesVariant);
    public static event RulesVariantChangedHandler RulesVariantChangedEvent;

    [Client]
    private void UpdatePlayerNumber(int playerNum)
    {
        if (NumberOfPlayersChangedEvent != null)
            NumberOfPlayersChangedEvent(playerNum);
    }

    [Client]
    private void UpdateDifficultyLevel(Difficulty difficulty)
    {
        if (DifficultyLevelChangedEvent != null)
            DifficultyLevelChangedEvent((int)difficulty);
    }

    [Client]
    private void UpdateCardVariant(CardVariant cardVariant)
    {
        if (CardVariantChangedEvent != null)
            CardVariantChangedEvent((int)cardVariant);
    }

    [Client]
    private void UpdateRulesVariant(RulesVariant rulesVariant)
    {
        if (RulesVariantChangedEvent != null)
            RulesVariantChangedEvent((int)rulesVariant);
    }
}