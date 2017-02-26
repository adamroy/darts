using UnityEngine;
using System.Collections;

public class InvestmentDart : DartBehavior
{
    protected override void OnDartHit()
    {
        var board = DartBoard.Instance;
        board.ScoreDart(this.gameObject, 0);
    }

    protected override void Finish()
    {
        var board = DartBoard.Instance;
        board.ScoreDart(this.gameObject, 3);
    }
}