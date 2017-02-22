using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class DartThrow : MonoBehaviour
{
    public float maxDistance;
    public float speedFactor;
    public float spin;
    [Range(0f, 1f)]
    public float minimumSteepnessFactor;
    public bool HasHitWall { get; private set; }

    #region Throw Modifier

    public interface IThrowModifier
    {
        Vector3 GetNewForward(Vector3 oldForward);
        bool ShouldStop();
    }

    private List<IThrowModifier> modifiers = new List<IThrowModifier>();

    public void AddThrowModifier(IThrowModifier mod)
    {
        modifiers.Add(mod);
    }

    public void RemoveThrowModifier(IThrowModifier mod)
    {
        modifiers.Remove(mod);
    }

    private Vector3 GetNewForward(Vector3 oldForward)
    {
        var currForward = oldForward;
        foreach (var mod in modifiers)
            currForward = mod.GetNewForward(currForward);
        return currForward;
    }

    private bool ShouldStop()
    {
        foreach (var mod in modifiers)
            if (mod.ShouldStop())
                return true;
        return false;
    }

    #endregion

    private void Throw(Vector3 power)
    {   
        StartCoroutine(ThrowCoroutine(power));
    }

    private void PlayNoise()
    {
        var sfx = new string[] { "woosh_low", "woosh_med", "woosh_high" };
        AudioManager.Play(sfx[Random.Range(0, sfx.Length)], 0.5f);
    }

    private IEnumerator ThrowCoroutine(Vector3 power)
    {
        PlayNoise();

        float t = power.magnitude;
        float distanceToTravel = maxDistance * t;
        float distanceAlreadyTravelled = 0;
        var forward = transform.forward;
        float steepnessFactor = Random.Range(minimumSteepnessFactor, 1);

        while (true)
        {
            if (ShouldStop() == false)
            {
                forward = GetNewForward(forward);
                var amountToMove = forward * speedFactor * Time.deltaTime;
                distanceAlreadyTravelled += amountToMove.magnitude;
                transform.Translate(amountToMove, Space.World);
                transform.Rotate(Vector3.forward, spin * Time.deltaTime, Space.Self);

                float progress = distanceAlreadyTravelled / distanceToTravel;
                float fudgedProgress = progress * steepnessFactor;
                float dx = forward.x * (1 - fudgedProgress);
                float dy = forward.y * (1 - fudgedProgress);
                transform.forward = new Vector3(dx, dy, fudgedProgress);
                SendMessage("SetHeight", 1 - progress);

                if (progress >= 1)
                    break;
            }
            else
            {
                float dx = forward.x * (1 - steepnessFactor);
                float dy = forward.y * (1 - steepnessFactor);
                transform.forward = new Vector3(dx, dy, steepnessFactor);
                break;
            }

            yield return null;
        }

        // Let the dart know it hit the wall
        this.SendMessage("HitWall");
        HasHitWall = true;
    }

}
