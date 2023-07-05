using UnityEngine;
using UnityEngine.SceneManagement;

// Třída pro načítání levelů a ukládání herního stavu
public static class LevelLoader
{
    // Názvy klíčů pro ukládání a načítání hodnot v PlayerPrefs
    public static string SCORE = nameof(SCORE);
    public static string HISCORE = nameof(HISCORE);
    public static string WAVE = nameof(WAVE);
    public static string LIVES = nameof(LIVES);

    // Výchozí hodnoty pro počet životů a nejvyšší dosažené skóre
    public static int DefaultLives = 3;
    public static int DefaultHiScore = 10000;

    // Metoda pro načtení levelu s daným stavem hry
    public static void LoadLevel(int wave, int score, int hiscore, int lives)
    {
        // Uložení hodnot do PlayerPrefs (systém pro ukládání dat mezi hraními sezeními)
        PlayerPrefs.SetInt(SCORE, score);
        PlayerPrefs.SetInt(HISCORE, hiscore);
        PlayerPrefs.SetInt(WAVE, wave);
        PlayerPrefs.SetInt(LIVES, lives);

        // Název scény, která se má načíst
        string sceneName = "Game";
        SceneManager.LoadScene(sceneName);
    }

    // Metoda pro spuštění nové hry s výchozími hodnotami
    public static void StartCleanGame()
    {
        LoadLevel(0, 0, DefaultHiScore, DefaultLives);
    }

    // Metoda pro získání aktuálního stavu hry (GameState) z uložených hodnot
    public static GameState GetGameState()
    {
        // Načtení hodnot ze PlayerPrefs
        int score = PlayerPrefs.GetInt(SCORE, 0);
        int hiscore = PlayerPrefs.GetInt(HISCORE, 0);
        int wave = PlayerPrefs.GetInt(WAVE, 0);
        int lives = PlayerPrefs.GetInt(LIVES, 0);

        // Vytvoření instance struktury GameState a nastavení hodnot
        return new GameState { score = score, lives = lives, hiscore = hiscore, wave = wave, type = GameStateType.Valid };
    }
}

// Výčtový typ pro různé stavy hry
public enum GameStateType
{
    Invalid, // Neplatný stav hry
    Valid    // Platný stav hry
}

// Struktura pro reprezentaci herního stavu
public struct GameState
{
    public int score;         // Aktuální skóre
    public int hiscore;       // Nejvyšší dosažené skóre
    public int wave;          // Aktuální kolo
    public int lives;         // Počet zbývajících životů
    public GameStateType type; // Typ herního stavu (platný nebo neplatný)
}
