using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    // Jednotlivé instance jedináčků jsou ukládány jako statické proměnné,
    // což zajišťuje, že jsou sdíleny mezi všemi instancemi třídy.
    private static T _instance;

    // Tato vlastnost umožňuje přístup k jedináčkovi přes Instance.
    public static T Instance
    {
        get
        {
            // Pokud dosud nebyla vytvořena žádná instance jedináčka,
            // najde se instance daného typu v rámci scény.
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<T>();
            }

            // Vrací se nalezená nebo již existující instance jedináčka.
            return _instance;
        }
    }
}
