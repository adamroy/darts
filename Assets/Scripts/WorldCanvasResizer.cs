using UnityEngine;
using System.Collections;

// Resizes a worldspace canvas to match the size of the given orthographic camera, height first
// Assumes camera is orthographic (which it is in our case)
[RequireComponent(typeof(Canvas))]
public class WorldCanvasResizer : MonoBehaviour
{
    public new Camera camera;

    private void Start()
    {
        // Should just need to do once at start since resolution on phone games doesn't change during play
        StartCoroutine(ResizeToFitCamera());
    }

    private IEnumerator ResizeToFitCamera()
    {
        var canvas = GetComponent<Canvas>();

        // This forces the canvas to fit the screen (also moves it but that's not a big problem)
        canvas.renderMode = RenderMode.ScreenSpaceCamera;

        // One frame lets it take effect
        yield return null;

        // Switch back to worldspace to that we can pan our camera around
        canvas.renderMode = RenderMode.WorldSpace;
    }
}