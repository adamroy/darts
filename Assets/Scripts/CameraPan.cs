using UnityEngine;
using System.Collections;

public class CameraPan : MonoBehaviour
{
    public Vector3 panDistance;
    public float panTime;
    public AnimationCurve panCurve;
    public bool IsPanning { get; private set; }
    public bool IsAtStart { get; private set; }

    private Vector3 startPosition;

    private void Start()
    {
        IsAtStart = true;
        startPosition = transform.position;
    }

    public void SnapTo(bool start, bool flipped = false)
    {
        var panDistance = this.panDistance * (flipped ? -1 : 1);
        IsAtStart = start;
        transform.position = start ? startPosition : startPosition + panDistance;
    }

    public Coroutine PanTo(bool toStart, bool flipped = false)
    {
        return StartCoroutine(PanToCoroutine(toStart, flipped));
    }

    private IEnumerator PanToCoroutine(bool toStart, bool flipped)
    {
        var panDistance = this.panDistance * (flipped ? -1 : 1);

        if (IsAtStart == toStart)
            yield break;
        IsPanning = true;

        Vector3 beginPos = toStart ? startPosition + panDistance : startPosition;
        Vector3 actualPos = transform.position;
        Vector3 endPos = toStart ? startPosition : startPosition + panDistance;

        // Calculate how far along we are so we can begin there
        float totalDist = Vector3.Distance(endPos, beginPos);
        float remainingDist = Vector3.Distance(endPos, actualPos);
        float t = 1 - remainingDist / totalDist;

        for (t *= panTime; t <= panTime; t += Time.deltaTime)
        {
            float p = panCurve.Evaluate(t / panTime);
            transform.position = Vector3.Lerp(beginPos, endPos, p);
            yield return null;
        }

        IsPanning = false;
        IsAtStart = toStart;
    }
}
