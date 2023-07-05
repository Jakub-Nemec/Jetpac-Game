using UnityEngine;

public class BouncingAlien : MonoBehaviour
{
    public float yGradient = 1f;
    public float xDirection = 1f;
    public float speed = 16;
    public GameObject explosion;

    private void Update()
    {
        // Pohybujeme objektem ve směru daném proměnnými xDirection a yGradient a rychlostí speed.
        transform.position += new Vector3(xDirection, yGradient) * speed * Time.deltaTime;

        if (Mathf.Abs(transform.position.x) > 185)
        {
            // Pokud narazíme na okraj obrazovky, přesuneme objekt na druhou stranu.
            float newX = -transform.position.x;
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            // Pokud se srazíme s platformou, změníme směr objektu ve vodorovném i svislém směru.
            xDirection *= -1;
            yGradient *= -1;
        }
        else if (collision.gameObject.tag == "Player")
        {
            // Pokud se srazíme s hráčem, vytvoříme explozi a zničíme tento objekt.
            var copy = Instantiate(explosion);
            copy.transform.position = transform.position;
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Laser")
        {
            // Pokud narazíme na laser, zvýšíme skóre hráče, vytvoříme explozi a zničíme tento objekt.
            ScoreManager.Instance.KillAlien();
            var copy = Instantiate(explosion);
            copy.transform.position = transform.position;
            Destroy(gameObject);
        }
    }

    internal void Init()
    {
        // Inicializace objektu.
        bool isRight = Random.Range(0f, 1f) > .5f;
        float x = isRight ? 180 : -180;
        float y = -88 + Random.Range(0f, 176);

        transform.position = new Vector3(x, y, 0);
        GetComponent<AlienInit>().flipAlien = isRight;

        xDirection = isRight ? -1 : 1;
        yGradient = -0.25f + Random.Range(0f, 0.5f);
        speed = 16 + Random.Range(0, 4f);
    }
}
