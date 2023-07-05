using UnityEngine;

public class FollowAlien : MonoBehaviour
{
    public GameObject explosion;

    private void Update()
    {
        // Kontrola, zda-li raketa je ve stavu "TakeOff". Pokud ano, ukončit funkci.
        if (RocketManager.Instance.State == RocketState.TakeOff) return;

        // Získání odkazu na transformaci hráče z herního manažera.
        Transform player = GameManager.Instance.ThePlayer.transform;

        // Vektor rozdílu mezi pozicí hráče a pozicí tohoto objektu normalizovaný.
        Vector3 diff = (player.transform.position - transform.position).normalized;

        // Pohyb objektu ve směru hráče s danou rychlostí.
        transform.position += diff * 16f * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kontrola, zda-li kolize je označena jako "Laser".
        if (collision.tag == "Laser")
        {
            // Zvýšení skóre hráče při zabití aliena.
            ScoreManager.Instance.KillAlien();

            // Vytvoření kopie exploze a umístění na pozici tohoto objektu.
            var copy = Instantiate(explosion);
            copy.transform.position = transform.position;

            // Zničení tohoto objektu.
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Kontrola, zda-li kolize je s objektem označeným jako "Player".
        if (collision.gameObject.tag == "Player")
        {
            // Vytvoření kopie exploze a umístění na pozici tohoto objektu.
            var copy = Instantiate(explosion);
            copy.transform.position = transform.position;

            // Zničení tohoto objektu.
            Destroy(gameObject);
        }
    }

    internal void Init()
    {
        // Generování náhodného směru (vpravo nebo vlevo).
        bool isRight = Random.Range(0f, 1f) > .5f;

        // Určení náhodných hodnot pro pozici objektu.
        float x = isRight ? 180 : -180;
        float y = -88 + Random.Range(0f, 176);

        // Nastavení pozice objektu na základě vygenerovaných hodnot.
        transform.position = new Vector3(x, y, 0);

        // Předání informace o směru objektu komponentě AlienInit.
        GetComponent<AlienInit>().flipAlien = isRight;
    }
}
