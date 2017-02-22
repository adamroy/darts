using UnityEngine;
using System.Collections;

public class DartBehavior : MonoBehaviour
{
    public string displayName;

    protected virtual string Sfx { get { return "dart_hit"; } }

    private void HitWall()
    {
        var board = DartBoard.Instance;
        if (board.IsDartHit(this.gameObject))
        {
            OnDartHit();
            AudioManager.Play(Sfx);
        }
        else
        {
            OnDartMiss();
            if (board.IsArmourSection(this.gameObject) == false)
                AudioManager.Play("dart_hit_wall");
            else
                AudioManager.Play("hit_armor");
            SendMessage("Fall");
        }
    }

    protected virtual void OnDartHit()
    {

    }

    protected virtual void OnDartMiss()
    {
         
    }
}
