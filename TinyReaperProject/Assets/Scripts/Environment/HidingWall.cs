using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class HidingWall : MonoBehaviour
{
    [SerializeField] private float _fadeOutTime = 0.2f;
    [SerializeField] private float _fadeInTime = 0.5f;

    private SpriteRenderer _spr;

    private Coroutine fadeIn;
    private Coroutine fadeOut;

    private void Start()
    {
        _spr = GetComponent<SpriteRenderer>();
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (fadeIn != null) StopCoroutine(fadeIn);
            fadeOut = StartCoroutine(Fade(targetAlpha: 0f, fadeTime: _fadeOutTime));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (fadeOut != null) StopCoroutine(fadeOut);
            fadeIn = StartCoroutine(Fade(targetAlpha: 1f, fadeTime: _fadeInTime));
        }
    }

    private IEnumerator Fade(float targetAlpha, float fadeTime)
    {
        Color initialColor = _spr.color;
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, targetAlpha);

        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            _spr.color = Color.Lerp(initialColor, targetColor, elapsedTime / fadeTime);
            yield return null;
        }
    }
}
