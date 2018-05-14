using UnityEngine.Networking;

[System.Serializable]
public class GameSettings : NetworkBehaviour
{
    public enum Difficulty { Easy = 0, Medium = 1, Hard = 2, Very_Hard = 3 }
    public enum CardVariant { Rhymes = 0, Numbers = 1 }
    public enum RulesVariant { Classic = 0, Solitaire = 1, Ultimate_Solitaire = 2, Goblins_Rule = 4 }

    [SyncVar]
    public int numberOfPlayers;
    [SyncVar]
    public Difficulty difficultyLevel;
    [SyncVar]
    public CardVariant cardVariant;
    [SyncVar]
    public RulesVariant rulesVariant;
}