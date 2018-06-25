using UnityEngine;

[System.Serializable]
public class GameSettings : MonoBehaviour
{
    public enum Difficulty { Easy = 0, Medium = 1, Hard = 2, VeryHard = 3 }
    public enum CardVariantType { Rhymes = 0, Numbers = 1 }
    public enum RulesVariantType { Classic = 0, Solitaire = 1, UltimateSolitaire = 2, GoblinsRule = 4 }

    public int numberOfPlayers;
    public Difficulty DifficultyLevel;
    public CardVariantType CardVariant;
    public RulesVariantType RulesVariant;
}