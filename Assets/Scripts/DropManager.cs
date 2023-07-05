using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropManager : SingletonMonoBehaviour<DropManager>
{
    // Maximální počet položek, které mohou být shozeny
    private static readonly int MAX_ITEMS = 2;
    // Hodnota x-ové souřadnice nejlevějšího okraje, ze kterého mohou být položky shozeny
    private static readonly float LEFT_MOST_EDGE = -112f;
    // Šířka oblasti, ve které mohou být položky shozeny
    private static readonly float DROP_WIDTH = 224f;

    // Příznak, zda se má shozeno palivo
    private bool _dropFuel = false;

    // Interval mezi shozením položek
    public float _dropRateSecs = 4f;

    // Příznak, zda je spuštěný proces shození položek
    public bool _isRunning = true;

    // Seznam shozených položek
    private List<GameObject> _droppedItems = new List<GameObject>();

    // Pole možných položek, které mohou být shozeny (bez paliva)
    public GameObject[] pickups;
    // Palivo
    public GameObject fuel;

    // Pravidla:
    //      Palivo je aktivní pouze pokud je raketa kompletní
    //      Může být shozeno pouze x položek / gemů
    //      Pokud je požadováno palivo a zbylá je jen jedna pozice, musí být shozeno palivo


    IEnumerator Start()
    {
        while (true)
        {
            // Přerušení na určitou dobu
            yield return new WaitForSeconds(_dropRateSecs);
            if (_isRunning && _droppedItems.Count < MAX_ITEMS)
            {
                if (RocketManager.Instance.State == RocketState.Fuelling)
                {
                    // Pokud je raketa v procesu tankování
                    // Určení, zda bude shozeno palivo nebo náhodná položka
                    GameObject prefab = _dropFuel ? fuel : pickups[Random.Range(0, pickups.Length)];
                    // Vytvoření kopie objektu
                    GameObject copy = Instantiate(prefab);
                    // Určení x-ové souřadnice shozené položky
                    float x = LEFT_MOST_EDGE + (DROP_WIDTH * Random.Range(0f, 1f));
                    // Nastavení pozice shozené položky
                    copy.transform.position = new Vector3(x, gameObject.transform.position.y);
                    // Přidání položky do seznamu shozených položek
                    _droppedItems.Add(copy);
                    // Přepnutí příznaku pro shození paliva
                    _dropFuel = !_dropFuel;
                }
                else if (RocketManager.Instance.State == RocketState.ReadyForTakeOff)
                {
                    // Pokud je raketa připravena k odletu
                    // Náhodný výběr položky, která bude shozena
                    GameObject prefab = pickups[Random.Range(0, pickups.Length)];
                    // Vytvoření kopie objektu
                    GameObject copy = Instantiate(prefab);
                    // Určení x-ové souřadnice shozené položky
                    float x = LEFT_MOST_EDGE + (DROP_WIDTH * Random.Range(0f, 1f));
                    // Nastavení pozice shozené položky
                    copy.transform.position = new Vector3(x, gameObject.transform.position.y);
                    // Přidání položky do seznamu shozených položek
                    _droppedItems.Add(copy);
                }
            }
        }
    }

    public void PickupObject(GameObject pickup)
    {
        // Odebrání položky ze seznamu shozených položek
        _droppedItems.Remove(pickup);

        // Získání komponenty Pickup ze shozené položky
        var p = pickup.GetComponent<Pickup>();
        // Pokud se nejedná o palivo, zničí se položka
        if (!p._isFuel)
        {
            Destroy(pickup);
        }
    }

    public void AddFuel(GameObject fuelPod)
    {
        // Odebrání paliva ze seznamu shozených položek
        _droppedItems.Remove(fuelPod);
        // Zničení paliva
        Destroy(fuelPod);

        // Zvýšení úrovně paliva rakety
        RocketManager.Instance._fuelLevel++;
    }
}