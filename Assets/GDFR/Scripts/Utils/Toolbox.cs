using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(GameSettings))]
public class Toolbox : NetworkBehaviour
{
    public const int MAX_NUMBER_PLAYERS = 4;
    public GameSettings gameSettings;

    private static Toolbox mInstance;

    public static Toolbox Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = FindObjectOfType<Toolbox>();
                if (mInstance == null)
                {
                    Debug.LogError("No Toolbox spawned at time of request");
                }
                return mInstance;
            }
            else
            {
                return mInstance;
            }
        }
    }

    //defaults
    public void LoadDefaultGameSettings()
    {
        gameSettings = gameObject.AddComponent<GameSettings>();
        gameSettings.numberOfPlayers = 4;
        gameSettings.difficultyLevel = GameSettings.Difficulty.Hard;
        gameSettings.cardVariant = GameSettings.CardVariant.Rhymes;
        gameSettings.rulesVariant = GameSettings.RulesVariant.Classic;
    }

    public PlayersProfile[] playerProfiles = new PlayersProfile[MAX_NUMBER_PLAYERS];
    //public int currentPlayerNumber = 1;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        LoadDefaultGameSettings();

        // Your initialization code here
        for (int i = 0; i < MAX_NUMBER_PLAYERS; i++)
        {
            playerProfiles[i] = new PlayersProfile();
        }
    }

    //// (optional) allow runtime registration of global objects
    //public static T RegisterComponent<T>() where T : Component
    //{
    //    return GetOrAddComponent<T>();
    //}
}

[System.Serializable]
public class PlayersProfile
{
    public enum Type { Human = 0, AI = 1 }

    public Type type = Type.Human;
    public string name = "";
    public int avatar = 0;
}