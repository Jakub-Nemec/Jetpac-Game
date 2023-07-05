using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneBehaviour : MonoBehaviour
{
    public float xDirection = 1f;
    public GameObject explosion;
    public AnimationCurve curve;
    public bool flyTowardsTarget;
    Vector3 _diff;


    // Spawns either side of the screen in the lower two thirds
    // Waits for a certain amount of time then swoops towards the player
    // Normal (fireball) alien rules apply - hittin a platform or the player kills it

    // Při kolizi s jiným objektem
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Platform" || collision.gameObject.tag == "Player")
        {
            var copy = Instantiate(explosion);
            copy.transform.position = transform.position;
            Destroy(gameObject);
        }
    }

    // Při vstupu do triggeru
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Laser")
        {
            ScoreManager.Instance.KillAlien();
            var copy = Instantiate(explosion);
            copy.transform.position = transform.position;
            Destroy(gameObject);
        }
    }

    // Aktualizace každý frame
    private void Update()
    {
        if (flyTowardsTarget)
        {
            transform.position += _diff * 16f * Time.deltaTime;
        }

        if (Mathf.Abs(transform.position.x) > 185)
            Destroy(gameObject);
    }

    // Inicializace objektu
    internal void Init()
    {
        bool isRight = Random.Range(0f, 1f) > .5f;
        float x = isRight ? 120 : -120;
        float y = -88 + Random.Range(0f, 176);

        transform.position = new Vector3(x, y, 0);
        GetComponent<AlienInit>().flipAlien = isRight;

        xDirection = isRight ? -1 : 1;
        StartCoroutine(WaitInFormation());
    }

    // Čekání v určené formaci
    IEnumerator WaitInFormation()
    {
        float time = 0f;

        while (time < 1.5f)
        {
            float y = curve.Evaluate(time);
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
            time += Time.deltaTime;
            yield return null;
        }

        Transform player = GameManager.Instance.ThePlayer.transform;
        _diff = (player.transform.position - transform.position).normalized;

        flyTowardsTarget = true;
    }
}
