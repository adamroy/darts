using UnityEngine;
using System.Collections;
using System;

public class MagneticDart : DartBehavior, DartThrow.IThrowModifier
{
    public float attractionForce;
    public float stopThreshold;

    void Start ()
    {
        SendMessage("AddThrowModifier", this);
	}

    protected override void OnDartHit()
    {
        var board = DartBoard.Instance;
        board.ScoreDart(this.gameObject);
    }

    public Vector3 GetNewForward(Vector3 oldForward)
    {
        var board = DartBoard.Instance;
        var accumulatedForce = Vector3.zero;
        int count = 0;

        foreach (var dart in board.Darts)
        {
            var deltaPos = dart.transform.position - this.transform.position;
            accumulatedForce += deltaPos.normalized * (attractionForce / Mathf.Pow(deltaPos.magnitude + 1, 2));
            count++;
        }

        if (count != 0)
        {
            var newForward = oldForward;
            newForward = oldForward + (accumulatedForce / count);
            newForward.Normalize();
            return newForward;
        }
        else
        {
            return oldForward;
        }
    }

    public bool ShouldStop()
    {
        var board = DartBoard.Instance;
        foreach (var dart in board.Darts)
        {
            var deltaPos = dart.transform.position - this.transform.position;
            if (deltaPos.magnitude < stopThreshold)
                return true;
        }

        return false;
    }
}
