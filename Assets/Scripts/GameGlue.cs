using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameGlue : MonoBehaviour
{
    public CameraPan splashPan, menuPan, randomizerPan, gamePan;
    public Fade blackFade, spashFade;
    public DartRandomizer dartRandomizer;
    public Gameplay gameplay;

    private void Start()
    {
        StartCoroutine(SplashCoroutine());
    }

    private IEnumerator SplashCoroutine()
    {
        splashPan.SnapTo(true);
        gamePan.SnapTo(false);
        randomizerPan.SnapTo(false);
        menuPan.SnapTo(false);

        yield return blackFade.FadeItem(false);

        yield return new WaitForSeconds(5);

        spashFade.FadeItem(false);
        AudioManager.Play("swoosh");
        splashPan.PanTo(false);
        yield return menuPan.PanTo(true);

        StartCoroutine(MenuCoroutine());
    }

    private IEnumerator MenuCoroutine()
    {
        while(!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }

        menuPan.PanTo(false);
        randomizerPan.PanTo(true);
        AudioManager.Play("swoosh");

        StartCoroutine(RandomizerCoroutine());
    }

    private IEnumerator RandomizerCoroutine()
    {
        dartRandomizer.Initialize();
        yield return null;

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0) == true);

        bool block = true;
        List<DartType> selectedDarts = null;
        dartRandomizer.Randomize((selec) => { selectedDarts = selec; block = false; });

        yield return new WaitWhile(() => block);

        yield return new WaitForSeconds(1f);

        randomizerPan.PanTo(false);
        gamePan.PanTo(true);
        AudioManager.Play("swoosh");

        StartCoroutine(GameCoroutine(selectedDarts));
    }

    private IEnumerator GameCoroutine(List<DartType> selectedDarts)
    {
        yield return gameplay.StartGame(selectedDarts);
        menuPan.SnapTo(false, true);
        AudioManager.Play("swoosh");
        var cr = gamePan.PanTo(false);
        yield return menuPan.PanTo(true, true);
        yield return cr;

        gameplay.Reset();
        StartCoroutine(MenuCoroutine());
    }
}
