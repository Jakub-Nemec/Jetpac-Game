using UnityEngine;

public class ScoreListener : MonoBehaviour
{
    public TMPro.TMP_Text _scoreIndicator;

    void Start()
    {
        // Přiřaďme akci k události PropertyChanged
        ScoreManager.Instance.PropertyChanged += (o, propertyName) =>
        {
            // Pokud se změnila vlastnost "Score"
            if (propertyName == "Score")
            {
                // Aktualizujeme textový objekt s hodnotou skóre
                _scoreIndicator.text = $"{ScoreManager.Instance.Score:000000}";
            }
        };
    }
}
