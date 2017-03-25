using UnityEngine;
using System.Collections;

public class SpinAndPop : MonoBehaviour
{
    public float spinTime;
    public int spins;
    public float popScale;
    public float popTime;

    public Coroutine Go()
    {
        return StartCoroutine(GoCoroutine());
    }

    private IEnumerator GoCoroutine()
    {
        for (float t = 0; t <= spinTime * spins; t += Time.deltaTime)
        {
            transform.localRotation = Quaternion.Euler(0, 0, (t / spinTime) * 360f);
            yield return null;
        }

        transform.localRotation = Quaternion.identity;

        transform.localScale = Vector3.one * popScale;
        yield return new WaitForSeconds(popTime);
        transform.localScale = Vector3.one;
    }
}
