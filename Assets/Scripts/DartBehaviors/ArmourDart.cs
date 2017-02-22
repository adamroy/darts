using UnityEngine;
using System.Collections;

public class ArmourDart : DartBehavior
{
    protected override void OnDartHit()
    {
        var board = DartBoard.Instance;
        board.ScoreDart(this.gameObject);
        board.ArmourSection(this.gameObject);
    }
}
