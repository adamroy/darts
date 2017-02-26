using UnityEngine;
using System.Collections;

public class RouletteDart : DartBehavior
{
    private bool firstHit = true;

    protected override void OnDartHit()
    {
        var board = DartBoard.Instance;
        board.ScoreDart(this.gameObject);

        if (firstHit == true)
        {
            firstHit = false;
            // board.RandomizeSection(this.gameObject);
            board.RandomSectionKnock(this.gameObject);
        }
    }
}
