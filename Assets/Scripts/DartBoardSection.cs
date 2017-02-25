using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class DartBoardSection : MonoBehaviour
{
    public int points;
    public new Collider2D collider;
    public Renderer armor;

    private bool isArmoured = false;
    public bool IsArmoured
    {
        get { return isArmoured; }
        set { isArmoured = value; SetArmourDisplay(value); }
    }

    [Serializable]
    public class MultiplierSection
    {
        public int multiplier;
        public Collider2D collider;
    }

    public List<MultiplierSection> multipliers;

    private void Start()
    {
        armor.enabled = false;
    }

    public int ScorePoints(Vector2 dartPosition)
    {
        int pointsScored = 0;

        if (collider.OverlapPoint(dartPosition))
        {
            pointsScored = this.points;

            foreach(var ms in multipliers)
            {
                if (ms.collider.OverlapPoint(dartPosition))
                    pointsScored *= ms.multiplier;
            }
        }

        return pointsScored;
    }

    public void Hit()
    {
        StartCoroutine(ScaleUpCoroutine(0.25f, 1.2f));
    }

    private IEnumerator ScaleUpCoroutine(float time, float factor)
    {
        var startScale = transform.localScale;
        transform.localScale = startScale * factor;

        yield return new WaitForSeconds(time);
        
        transform.localScale = startScale;
    }

    private void SetArmourDisplay(bool isArmoured)
    {
        AudioManager.Play("armor", delay: 0.5f);
        armor.enabled = true;
        StartCoroutine(ArmourCoroutine());
    }

    private IEnumerator ArmourCoroutine()
    {
        float time = 1f;

        while (true)
        {
            for (float t = 0, p = 0; t < time; t += Time.deltaTime, p = t / time)
            {
                armor.material.SetFloat("_AnimationT", 1f - 2f * p);
                yield return null;
            }

            armor.material.SetFloat("_AnimationT", -1f);

            yield return new WaitForSeconds(5f);
        }
    }
}
