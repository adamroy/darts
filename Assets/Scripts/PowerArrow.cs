using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PowerArrow : MonoBehaviour
{
    private static PowerArrow instance;

    public float minHeight, maxHeight;
    public Transform pivotPoint;

    private Image arrow;
    private float currentAngle;

    public static void TurnOffDisplay()
    {
        instance.arrow.enabled = false;
    }

    public static void DisplayPower(Vector2 power)
    {
        instance.m_DisplayPower(power);
    }

    private void m_DisplayPower(Vector2 power)
    {
        arrow.enabled = power != Vector2.zero;

        float targetAngle = Mathf.Atan2(power.y, power.x) * Mathf.Rad2Deg - 90f;
        float deltaAngle = targetAngle - currentAngle;
        currentAngle = targetAngle;
        transform.RotateAround(pivotPoint.position, Vector3.forward, deltaAngle);

        var size = arrow.rectTransform.sizeDelta;
        size.y = Mathf.Lerp(minHeight, maxHeight, Mathf.Pow(power.magnitude, 2));
        arrow.rectTransform.sizeDelta = size;
    }

    private void Start()
    {
        arrow = GetComponent<Image>();
        instance = this;
        TurnOffDisplay();
    }

    private void OnDestroy()
    {
        instance = null;
    }
}
