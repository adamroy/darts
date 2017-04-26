using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashScreen : MonoBehaviour
{
    public float loadDelay;
    public CameraPan pan;

    private void Start()
    {
        StartCoroutine(SplashCoroutine());
	}

    private IEnumerator SplashCoroutine()
    {
        yield return new WaitForSeconds(loadDelay);
    }
}
