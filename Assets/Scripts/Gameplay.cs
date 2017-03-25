using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class Gameplay : MonoBehaviour
{
    public DartBoard dartboard;
    public Transform dartSpawnLocation;
    public Text dartDisplayName;
    public ScoreDisplay p1ScoreDisplay, p2ScoreDisplay;
    public DartSelector playerOneDarts, playerTwoDarts;

    private int p1Score, p2Score;
    private bool quit = false;
    private GameObject currentDart = null;

    private void Start()
    {
        DartBoard.Instance.OnPointsScored += OnPlayerScored;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            QuitSession();
    }

    private void OnPlayerScored(GameObject dart, int points)
    {
        if (dart.GetComponent<DartOwner>().player == DartOwner.Player.one)
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

    public Coroutine StartGame(List<DartType> selectedDarts)
    {
        return StartCoroutine(GameCoroutine(selectedDarts));
    }

    public void QuitSession()
    {
        quit = true;
    }

    public void Reset()
    {
        dartboard.Reset();
        if(currentDart !=null)
            Destroy(currentDart);
        currentDart = null;
        quit = false;
        p1Score = p2Score = 0;
        p1ScoreDisplay.Reset();
        p2ScoreDisplay.Reset();
        playerOneDarts.Reset();
        playerTwoDarts.Reset();
    }

    private IEnumerator GameCoroutine(List<DartType> selectedDarts)
    {
        quit = false;

        // Assign selected dart to buttons
        int i = 0;
        foreach (var dart in selectedDarts)
        {
            playerOneDarts.buttons[i].dart = dart.playerOnePrefab;
            playerOneDarts.buttons[i].button.image.sprite = dart.buttonImage;

            playerTwoDarts.buttons[i].dart = dart.playerTwoPrefab;
            playerTwoDarts.buttons[i].button.image.sprite = dart.buttonImage;

            i++;
        }

        bool playerOneTurn = true;
        GameObject currentDartPrefab = null;
        Action<GameObject> setDart = (GameObject dart) =>
        {
            if (currentDart != null) Destroy(currentDart);

            currentDartPrefab = dart;
            currentDart = Instantiate(dart, dartSpawnLocation.position, dartSpawnLocation.rotation) as GameObject;
            currentDart.SendMessage("Init", dartSpawnLocation);
            //currentDart.transform.SetParent(this.transform);
            dartDisplayName.text = currentDart.GetComponent<DartBehavior>().displayName;
        };

        playerOneDarts.OnDartSelected += setDart;
        playerTwoDarts.OnDartSelected += setDart;

        Action quitAction = () =>
        {
            playerOneDarts.OnDartSelected -= setDart;
            playerTwoDarts.OnDartSelected -= setDart;
        };

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

            yield return new WaitUntil(() => currentDart == null || currentDart.GetComponent<DartThrow>().HasHitWall || quit == true);

            if (quit)
            {
                quitAction();
                yield break;
            }

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
