using UnityEngine;
using System.Collections;

public class ClownDart : DartBehavior
{
    public AudioClip[] hitClips;
    public AudioClip[] missClips;

    private static int index = -1;

    private static int GetNextIndex(int length)
    {
        if (index < 0) index = Random.Range(0, length);
        index = (index + Random.Range(0, length - 1)) % length;
        return index;
    }

    protected override void OnDartHit()
    {
        var board = DartBoard.Instance;
        board.ScoreDart(this.gameObject);
        var clip = hitClips[Random.Range(0, hitClips.Length)];
        AudioManager.Play(clip);
    }

    protected override void OnDartMiss()
    {
        var clip = missClips[GetNextIndex(missClips.Length)];
        AudioManager.Play(clip);
    }
}
