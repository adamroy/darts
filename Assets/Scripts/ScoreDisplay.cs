using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreDisplay : MonoBehaviour
{
    public Text scoreText;
    public float animationTime;
    public float animationHeight;
    [Range(0f, 1f)]
    public float volume;

    private int frameID;
    private int deltaScore;
    private int score;

    void Start()
    {
        frameID = -1;
        deltaScore = 0;
    }

    public void ChangeScore(int amount)
    {
        frameID = Time.frameCount;
        deltaScore += amount;
    }

    private void LateUpdate()
    {
        if (frameID == Time.frameCount)
        {
            StartCoroutine(ChangeScoreCoroutine(deltaScore));
            deltaScore = 0;
            frameID = -1;
        }
    }

    private IEnumerator ChangeScoreCoroutine(int amount)
    {
        if (amount == 0) yield break;
        score += amount;
        var addedScore = Instantiate(this.scoreText.gameObject);
        addedScore.transform.SetParent(scoreText.transform.parent, true);
        addedScore.transform.localScale = Vector3.one;
        addedScore.GetComponent<Text>().text = (amount > 0 ? "+" : "-") + Mathf.Abs(amount);
        var color = addedScore.GetComponent<Text>().color;
        color.a = 0f;
        addedScore.GetComponent<Text>().color = color;
        addedScore.transform.position = scoreText.transform.position;
        var startPos = addedScore.transform.position;

        float height = animationHeight * Mathf.Sign(amount);
        float v0 = 4f * height / animationTime;
        float a = -8f * height / (animationTime * animationTime);
        
        if (amount > 0) AudioManager.Play("cha", volume);
        else AudioManager.Play("cha_neg", volume);

        for (float t = 0; t < animationTime; t += Time.deltaTime)
        {
            float h = 0.5f * a * t * t + v0 * t;
            addedScore.transform.position = startPos + Vector3.up * h;
            color.a = Mathf.Abs(h) / animationHeight;
            addedScore.GetComponent<Text>().color = color;
            yield return null;
        }

        if (amount > 0) AudioManager.Play("ching", volume);
        else AudioManager.Play("ching_neg", volume);

        Destroy(addedScore);
        scoreText.text = "<b>" + score + "</b>";
    }
}
