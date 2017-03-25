using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameGlue : MonoBehaviour
{
    public Transform dartSpawnLocation;
    public DartSelector playerOneDarts, playerTwoDarts;
    public ScoreDisplay p1ScoreDisplay, p2ScoreDisplay;
    public Text dartDisplayName;
    public DartBoard dartboard;
    public CameraPan menuPan, randomizerPan, gamePan;
    public DartRandomizer dartRandomizer;

    private int p1Score, p2Score;

    private void Start()
    {
        DartBoard.Instance.OnPointsScored += OnPlayerScored;
        StartCoroutine(MenuCoroutine());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene(0);
    }

    private void OnPlayerScored(GameObject dart, int points)
    {
        if(dart.GetComponent<DartOwner>().player == DartOwner.Player.one)
        {
            p1ScoreDisplay.ChangeScore(points);
            p1Score += points;
        }
        else
        {
            p2ScoreDisplay.ChangeScore(points);
            p2Score += points;
        }
    }

    private IEnumerator MenuCoroutine()
    {
        gamePan.SnapTo(false);
        randomizerPan.SnapTo(false);

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
        // Assign selected dart to buttons
        int i = 0; 
        foreach(var dart in selectedDarts)
        {
            playerOneDarts.buttons[i].dart = dart.playerOnePrefab;
            playerOneDarts.buttons[i].button.image.sprite = dart.buttonImage;

            playerTwoDarts.buttons[i].dart = dart.playerTwoPrefab;
            playerTwoDarts.buttons[i].button.image.sprite = dart.buttonImage;

            i++;
        }

        bool playerOneTurn = true;
        GameObject currentDartPrefab = null, currentDart = null;
        Action<GameObject> setDart = (GameObject dart) =>
        {
            if (currentDart != null) Destroy(currentDart);
            
            currentDartPrefab = dart;
            currentDart = Instantiate(dart, dartSpawnLocation.position, dartSpawnLocation.rotation) as GameObject;
            currentDart.SendMessage("Init", dartSpawnLocation);
            dartDisplayName.text = currentDart.GetComponent<DartBehavior>().displayName;
        };

        playerOneDarts.OnDartSelected += setDart;
        playerTwoDarts.OnDartSelected += setDart;

        while (true)
        {
            // Let the dartboard finish activites such as roulette
            yield return dartboard.AwaitActivities();

            playerOneDarts.SetEnabled(playerOneTurn);
            playerTwoDarts.SetEnabled(!playerOneTurn);
            var currentPlayerDarts = playerOneTurn ? playerOneDarts : playerTwoDarts;

            // Get the next available dart as a default
            var nextDart = currentPlayerDarts.NextDart;
            if (nextDart != null)
            {
                setDart(nextDart);
                currentPlayerDarts.SetSelectedDart(nextDart);
            }
            else
            {
                // If there are no more available darts, we've finished the game!
                break;
            }

            yield return new WaitUntil(() => currentDart == null || currentDart.GetComponent<DartThrow>().HasHitWall);

            // Let the dart tracker know we've used a dart
            currentPlayerDarts.DartUsed(currentDartPrefab);
            currentDartPrefab = null;
            currentDart = null;

            playerOneTurn = !playerOneTurn;
        }
        
        playerOneDarts.SetEnabled(false);
        playerTwoDarts.SetEnabled(false);

        yield return new WaitForSeconds(2f);

        dartboard.FinalizeScore();
        dartDisplayName.text = "Game Over";
    }
}
