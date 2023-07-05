using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject regularMenu; // běžné menu
    public GameObject debugMenu; // debugovací menu
    public Button[] levelButtons; // tlačítka úrovní
    public Sprite[] waveGraphic; // grafika vln
    public GameObject debugMenuButton; // tlačítko pro debugovací menu
    public Transform panel; // panel

    void Start()
    {
        for (int index = 0; index < 16; index++)
        {
            GameObject menuButton = Instantiate(debugMenuButton, panel); // vytvoření tlačítka menu
            Button button = menuButton.GetComponent<Button>(); // získání komponenty tlačítka
            TMPro.TextMeshProUGUI txt = button.GetComponentInChildren<TMPro.TextMeshProUGUI>(); // získání komponenty pro text
            Image img = menuButton.GetComponentInChildren<Image>(); // získání komponenty pro obrázek
            txt.text = $"Úroveň {index + 1}"; // nastavení textu tlačítka
            img.sprite = waveGraphic[index % 8]; // pouze 8 různých vetřelců, ale 16 obrazovek
            int levelIndex = index;
            button.onClick.AddListener(() =>
            {
                LevelLoader.LoadLevel(levelIndex, 0, 0, LevelLoader.DefaultLives); // načtení úrovně
            });
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            debugMenu.SetActive(!debugMenu.activeInHierarchy); // přepnutí viditelnosti debugovacího menu
            regularMenu.SetActive(!debugMenu.activeInHierarchy); // přepnutí viditelnosti běžného menu
        }
    }
}
