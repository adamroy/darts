using UnityEngine;
using System.Collections;

public class SlideDart : DartBehavior
{
    public float slideAmount;

    protected override void OnDartHit()
    {
        var board = DartBoard.Instance;
        board.ScoreDart(this.gameObject);
        board.IncreaseSlide(slideAmount);
    }
}
