using UnityEngine;
using System.Collections;

public class SpinDart : DartBehavior
{
    public float rotationAmount;

    protected override void OnDartHit()
    {
        var board = DartBoard.Instance;
        board.ScoreDart(this.gameObject);
        board.IncreaseRotation(rotationAmount);
    }
}
