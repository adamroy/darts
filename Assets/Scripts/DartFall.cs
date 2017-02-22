using UnityEngine;
using System.Collections;

public class DartFall : MonoBehaviour
{
    public float timeToDestroy;
    public float maxRotation;
    public float acceleration;
    public float destroyHeight;
    public float soundDelay;

    private bool isFalling;

    private void Fall()
    {
        transform.SetParent(null, true);
        isFalling = true;
        StartCoroutine(FallCoroutine());
    }
    
    private void Update()
    {
        // In case user throws the dart straight down
        if(!isFalling && transform.position.y < destroyHeight)
        {
            AudioManager.Play("dart_hit_floor");
            Destroy(this.gameObject);
        }
    }

    private IEnumerator FallCoroutine()
    {
        float angle = Random.Range(0.5f, 1f) * maxRotation;
        var axis = Random.insideUnitSphere;
        float velocity = 0;
        float height = 0f;
        float heightGainRate = 1f / Mathf.Sqrt(2f * (destroyHeight - transform.position.y) / -acceleration);

        while (transform.position.y > destroyHeight)
        {
            SendMessage("SetHeight", height);
            height += heightGainRate * Time.deltaTime;
            var rotation = Quaternion.AngleAxis(angle * Time.deltaTime, axis);
            transform.rotation *= rotation;
            velocity += acceleration * Time.deltaTime;
            transform.Translate(Vector3.down * velocity * Time.deltaTime, Space.World);

            yield return null;
        }

        AudioManager.Play("dart_hit_floor", delay: Random.value * soundDelay);
        Destroy(this.gameObject);
    }
}
