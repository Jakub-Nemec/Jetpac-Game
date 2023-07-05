using UnityEngine;

public class HiScoreListener : MonoBehaviour
{
    public TMPro.TMP_Text _scoreIndicator;  // Textový prvek pro zobrazení skóre

    void Start()
    {
        // Nastaví počáteční hodnotu skóre na indikátoru
        _scoreIndicator.text = $"{ScoreManager.Instance.HiScore:000000}";

        // Přiřadí událost (event) PropertyChanged z třídy ScoreManager
        ScoreManager.Instance.PropertyChanged += (o, propertyName) =>
        {
            // Pokud se změní vlastnost "HiScore"
            if (propertyName == "HiScore")
            {
                // Pokud aktuální skóre je větší nebo rovno nejlepšímu skóre
                if (ScoreManager.Instance.Score >= ScoreManager.Instance.HiScore)
                {
                    int hiScore = ScoreManager.Instance.HiScore;
                    // Aktualizuje zobrazení nejlepšího skóre na indikátoru
                    _scoreIndicator.text = $"{hiScore:000000}";
                }
            }
        };
    }
}