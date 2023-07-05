using UnityEngine;
using Random = UnityEngine.Random;

public class FireballAlien : MonoBehaviour
{
    public float yGradient = 1f; // Sklon pohybu aliena ve směru osy Y
    public float xDirection = 1f; // Směr pohybu aliena ve směru osy X
    public float speed = 16; // Rychlost pohybu aliena
    public GameObject explosion; // Prefab exploze

    // Metoda volaná při kolizi s jiným 2D objektem
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Pokud došlo ke kolizi s objektem s tagem "Platform" nebo "Player"
        if (collision.gameObject.tag == "Platform" || collision.gameObject.tag == "Player")
        {
            var copy = Instantiate(explosion); // Vytvoření kopie exploze
            copy.transform.position = transform.position; // Nastavení pozice exploze na pozici aliena
            Destroy(gameObject); // Zničení aliena
        }
    }

    // Metoda volaná při trigger kolizi s jiným 2D colliderem
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Pokud došlo ke kolizi s objektem s tagem "Laser"
        if (collision.tag == "Laser")
        {
            ScoreManager.Instance.KillAlien(); // Zvýšení skóre hráče
            var copy = Instantiate(explosion); // Vytvoření kopie exploze
            copy.transform.position = transform.position; // Nastavení pozice exploze na pozici aliena
            Destroy(gameObject); // Zničení aliena
        }
    }

    // Metoda volaná každý frame
    private void Update()
    {
        // Pohyb aliena ve směru osy X a Y s danou rychlostí
        transform.position += new Vector3(xDirection, yGradient) * speed * Time.deltaTime;

        // Pokud je absolutní hodnota pozice aliena na ose X větší než 185
        if (Mathf.Abs(transform.position.x) > 185)
            Destroy(gameObject); // Zničení aliena
    }

    // Inicializace aliena
    internal void Init()
    {
        // Náhodné rozhodnutí, zda se alien pohybuje vpravo nebo vlevo
        bool isRight = Random.Range(0f, 1f) > .5f;
        float x = isRight ? 180 : -180; // Pozice aliena na ose X
        float y = -88 + Random.Range(0f, 176); // Pozice aliena na ose Y

        transform.position = new Vector3(x, y, 0); // Nastavení pozice aliena
        GetComponent<AlienInit>().flipAlien = isRight; // Nastavení flipování aliena

        // Nastavení směru pohybu aliena ve směru osy X a sklonu pohybu ve směru osy Y s náhodnými hodnotami
        xDirection = isRight ? -1 : 1;
        yGradient = -0.25f + Random.Range(0f, 0.5f);
        speed = 16 + Random.Range(0, 4f); // Nastavení rychlosti pohybu aliena
    }
}
