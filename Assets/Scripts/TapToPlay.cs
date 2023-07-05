using UnityEngine;

public class TapToPlay : MonoBehaviour
{
    public string sceneName = "Game";

    void Start()
    {
        // Získání komponenty Button připojené ke hře objektu
        var button = GetComponent<UnityEngine.UI.Button>();

        // Přidání posluchače události kliknutí na tlačítko
        button.onClick.AddListener(new UnityEngine.Events.UnityAction(() =>
        {
            // Spuštění načítání herního levelu
            LevelLoader.StartCleanGame();
        }));
    }
}