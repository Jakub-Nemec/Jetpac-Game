using UnityEngine;

public class FuelDropTrigger : MonoBehaviour
{
    public DropManager _dropManager;

    // Metoda spuštěná, když dojde ke kolizi s jiným objektem
    public void OnTriggerEnter2D(Collider2D collision)
    {
        // Pokud kolize nastane s objektem označeným jako "Dropped Fuel"
        if (collision.tag == "Dropped Fuel")
        {
            // Přidáme palivo do správce paliva (_dropManager)
            _dropManager.AddFuel(collision.gameObject);
        }
    }

}
