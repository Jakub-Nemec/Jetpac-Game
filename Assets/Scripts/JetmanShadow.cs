using System.Collections;
using UnityEngine;

public class JetmanShadow : MonoBehaviour
{
    private Locomotion _player; // Hráč
    private Vector3 _offset; // Odsazení
    public bool isRunning; // Běží

    public void SetPlayer(Locomotion player, Vector3 offset) // Nastavení hráče a odsazení
    {
        _player = player;
        _offset = offset;
        transform.localPosition = _offset;

        if (_player == null)
        {
            GetComponent<SpriteRenderer>().sprite = null;
        }
    }

    IEnumerator Start() // Spuštění
    {
        while (true)
        {
            if (_player != null)
            {
                Sprite sprite = _player.jetmanShape._sprites[_player.jetmanShape.currentFrame]; // Sprite hráče
                SpriteRenderer renderer = GetComponent<SpriteRenderer>(); // Renderer
                renderer.sprite = sprite; // Nastavení sprite
                renderer.flipX = _player.jetmanShape.FlipHorizontal; // Otočení sprite horizontálně
            }

            yield return null;
        }
    }
}
