using UnityEngine;

public class Toolbox : Singleton<Toolbox>
{
    protected Toolbox() { } // guarantee this will be always a singleton only - can't use the constructor!

    public const int MAX_NUMBER_PLAYERS = 4;

    public GameSettings gameSettings = new GameSettings();
    public PlayersProfile[] playerProfiles = new PlayersProfile[MAX_NUMBER_PLAYERS];
    //public int currentPlayerNumber = 1;

    void Awake()
    {
        // Your initialization code here
        for (int i = 0; i < MAX_NUMBER_PLAYERS; i++)
        {
            playerProfiles[i] = new PlayersProfile();
        }
    }

    // (optional) allow runtime registration of global objects
    static public T RegisterComponent<T>() where T : Component
    {
        return Instance.GetOrAddComponent<T>();
    }
}

[System.Serializable]
public class GameSettings
{
    public enum Difficulty { Easy = 0, Medium = 1, Hard = 2, Very_Hard = 3 }
    public enum CardVariant { Rhymes = 0, Numbers = 1 }
    public enum RulesVariant { Classic = 0, Solitaire = 1, Ultimate_Solitaire = 2, Goblins_Rule = 4 }

    public int numberOfPlayers = 1;
    public Difficulty difficultyLevel = Difficulty.Easy;
    public CardVariant cardVariant = CardVariant.Rhymes;
    public RulesVariant rulesVariant = RulesVariant.Solitaire;
}

[System.Serializable]
public class PlayersProfile
{
    public enum Type { Human = 0, AI = 1 }

    public Type type = Type.Human;
    public string name = "";
    public int avatar = 0;
}