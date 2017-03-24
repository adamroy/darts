using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class DartRandomizer : MonoBehaviour
{
    [Serializable]
    public class WeightedDartType
    {
        public DartType dart;
        public float weight;
    }

    public List<WeightedDartType> darts;
    public List<Image> displayImages; // <- determines the number of darts selected
    public float randomizationTime;
    public float timeBetweenSwitches;

    private List<DartType> selectedDarts;

    public void Initialize()
    {
        selectedDarts = new List<DartType>();
    }

    public void Randomize(Action<List<DartType>> callback)
    {
        StartCoroutine(RandomizeCoroutine(callback));
    }

    private DartType SelectRandomDart()
    {
        float sum = darts.Aggregate(0f, (acc, d) => acc + d.weight);
        float selec = Random.Range(0, sum);
        float accum = 0f;
        foreach(var d in darts)
        {
            if (accum < selec && selec <= accum + d.weight)
                return d.dart;
            accum += d.weight;
        }

        return null;
    }

    private IEnumerator RandomizeCoroutine(Action<List<DartType>> callback)
    {
        foreach (var di in displayImages)
        {
            var dart = SelectRandomDart();
            selectedDarts.Add(dart);
            di.sprite = dart.buttonImage;
        }

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
                    var dart = SelectRandomDart();
                    selectedDarts[j] = dart;
                    displayImages[j].sprite = dart.buttonImage;
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
