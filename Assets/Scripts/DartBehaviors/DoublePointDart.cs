using UnityEngine;
using System.Collections;

public class DoublePointDart : DartBehavior
{
    protected override void OnDartHit()
    {
        var board = DartBoard.Instance;
        board.ScoreDart(this.gameObject, 2);
    }
}
