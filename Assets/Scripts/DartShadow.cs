using UnityEngine;
using System.Collections;

// Generates a shadow and scales it based on updated dart "height"
public class DartShadow : MonoBehaviour
{
    public GameObject shadowPrefab;

    private GameObject shadow;

    private void Start()
    {
        shadow = Instantiate(shadowPrefab);
        shadow.name = this.name + "_shadow";
        SetHeight(1);
    }

    private void Update()
    {
        shadow.transform.position = this.GetComponentInChildren<Renderer>().bounds.center + Vector3.forward * 2f;
    }

    // Height is between 1 (max height) and 0 (min height)
    public void SetHeight(float h)
    {
        shadow.transform.localScale = Vector3.one * (1 - Mathf.Clamp01(h));
    }

    public void OnDestroy()
    {
        Destroy(shadow);
    }
}
