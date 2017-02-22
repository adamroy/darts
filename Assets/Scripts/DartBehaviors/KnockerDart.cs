using UnityEngine;
using System.Collections;

public class KnockerDart : DartBehavior
{
    protected override string Sfx { get { return "dart_knock"; } }

    protected override void OnDartHit()
    {
        var board = DartBoard.Instance;
        board.ScoreDart(this.gameObject);
        board.KnockSection(this.gameObject);
    }
}
