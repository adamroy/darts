using UnityEngine;
using System.Collections;

// Resizes a worldspace canvas to match the size of the given orthographic camera, height first
// Assumes camera is orthographic (which it is in our case)
[RequireComponent(typeof(Canvas))]
public class WorldCanvasResizer : MonoBehaviour
{
    public new Camera camera;

    private void Awake()
    {
        // Should just need to do once at start since resolution on phone games doesn't change during play
        ResizeToFitCamera();
    }

    private void ResizeToFitCamera()
    {
        var rectTransform = GetComponent<RectTransform>();
        float aspect = Screen.width / (float)Screen.height;

        var canvasSize = rectTransform.sizeDelta;
        canvasSize.x = canvasSize.y * aspect;
        rectTransform.sizeDelta = canvasSize;
    }
}