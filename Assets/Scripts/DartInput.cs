using UnityEngine;
using System.Collections;
using System;

public class DartInput : MonoBehaviour
{
    public float touchActivationRadius;
    public float touchMaxRadius;
    public float maxViewRadius;
    [Range(0f, 1f)]
    public float deadPercent;
    public float centerOffset;

    private Transform center;
    private Vector3 startingWorldPos;
    private Quaternion startingWorldRot;

    private void Init(Transform initialTransform)
    {
        // Attach the dart to a "handle" GO that is at its center
        center = new GameObject().transform;
        center.name = "center";
        center.position = GetComponentInChildren<Renderer>().bounds.center + Vector3.up * centerOffset;
        center.rotation = initialTransform.rotation;
        transform.SetParent(center, true);
        center.position = initialTransform.position;

        startingWorldPos = center.position;
        startingWorldRot = center.rotation;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && IsInputInsideTouchRadius())
        {
            StartCoroutine(InputCoroutine());
        }
    }

    private bool IsInputInsideTouchRadius()
    {
        return Vector3.Distance(GetInputPosition(), startingWorldPos) < touchActivationRadius;
    }

    // World position with depth at dart depth
    private Vector3 GetInputPosition()
    {
        var inputPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        inputPos.z = center.position.z;
        return inputPos;
    }

    private IEnumerator InputCoroutine()
    {
        Vector3 power = Vector3.zero;

        while(Input.GetMouseButton(0))
        {
            var deltaPos = GetInputPosition() - startingWorldPos;
            float t = Mathf.Clamp01(deltaPos.magnitude / touchMaxRadius);

            // Cutoff at low power to give player option to cancel
            if (t < deadPercent)
            {
                center.position = startingWorldPos;
                center.rotation = startingWorldRot;
                power = Vector3.zero;
            }
            else
            {
                power = -deltaPos.normalized * t;
                float vt = Mathf.Sqrt(t);
                float viewDelta = maxViewRadius * vt;
                center.position = startingWorldPos + deltaPos.normalized * viewDelta;
                center.rotation = Quaternion.LookRotation(-deltaPos);
            }

            PowerArrow.DisplayPower(power);

            yield return null;
        }

        if (power != Vector3.zero)
        {
            transform.SetParent(null);
            Destroy(center.gameObject);
            SendMessage("Throw", power);
            // Prevent dart from recieving new inputs
            this.enabled = false;
        }

        PowerArrow.TurnOffDisplay();
    }
}
