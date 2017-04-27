using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Fade : MonoBehaviour
{
    public float time;
    public bool disableOnInvisible;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public Coroutine FadeItem(bool alpha)
    {
        gameObject.SetActive(true);
        return StartCoroutine(FadeCoroutine(alpha));
    }
    
    private IEnumerator FadeCoroutine(bool alpha)
    {
        for (float t = 0, p = 0; t <= time; t += Time.deltaTime, p = t / time)
        {
            var c = image.color;
            c.a = alpha ? p : 1 - p;
            image.color = c;

            yield return null;
        }

        var cl = image.color;
        cl.a = alpha ? 1 : 0;
        image.color = cl;

        if (alpha == false && disableOnInvisible == true)
        {
            gameObject.SetActive(false);
        }
    }
}
