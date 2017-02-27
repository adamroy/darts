using UnityEngine;
using System.Collections;

public class ClownDart : DartBehavior
{
    public AudioClip[] hitClips;
    public AudioClip[] missClips;

    protected override void OnDartHit()
    {
        var board = DartBoard.Instance;
        board.ScoreDart(this.gameObject);
        var clip = hitClips[Random.Range(0, hitClips.Length)];
        AudioManager.Play(clip);
    }

    protected override void OnDartMiss()
    {
        var clip = missClips[Random.Range(0, missClips.Length)];
        AudioManager.Play(clip);
    }
}
