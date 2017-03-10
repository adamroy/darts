using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class DartRandomizer : MonoBehaviour
{
    public List<DartType> darts;
    public List<Image> displayImages; // <- determines the number of darts selected
    public float randomizationTime;
    public float timeBetweenSwitches;

    private List<DartType> selectedDarts;

    public void Initialize()
    {
        selectedDarts = new List<DartType>();

        foreach(var di in displayImages)
        {
            int choice = Random.Range(0, darts.Count);
            selectedDarts.Add(darts[choice]);
            di.sprite = darts[choice].buttonImage;
        }
    }

    public void Randomize(Action<List<DartType>> callback)
    {
        StartCoroutine(RandomizeCoroutine(callback));
    }

    private IEnumerator RandomizeCoroutine(Action<List<DartType>> callback)
    {
        float timePerSelection = randomizationTime / displayImages.Count;
        int i = 0;
        float time = 0f;
        float switchTime = 0f;

        while (i < displayImages.Count)
        {
            if (time > i * timePerSelection)
                i++;

            if (switchTime > timeBetweenSwitches)
            {
                for (int j = i; j < displayImages.Count; j++)
                {
                    int choice = Random.Range(0, darts.Count);
                    selectedDarts[j] = darts[choice];
                    displayImages[j].sprite = darts[choice].buttonImage;
                }

                AudioManager.Play("click");

                switchTime = 0f;
            }

            switchTime += Time.deltaTime;
            time += Time.deltaTime;

            yield return null;
        }

        callback(selectedDarts);
    }
}
