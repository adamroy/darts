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

    private int p1Score, p2Score;

    private void Start()
    {
        DartBoard.Instance.OnPointsScored += OnPlayerScored;
        StartCoroutine(MainCoroutine());
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

    private IEnumerator MainCoroutine()
    {
        bool playerOneTurn = true;
        GameObject currentDartPrefab = null, currentDart = null;
        Action<GameObject> setDart = (GameObject dart) =>
        {
            if (currentDart != null) Destroy(currentDart);
            
            currentDartPrefab = dart;
            currentDart = Instantiate(dart, dartSpawnLocation.position, dartSpawnLocation.rotation) as GameObject;
            currentDart.SendMessage("Init", dartSpawnLocation);
            dartDisplayName.text = "<b>" + currentDart.GetComponent<DartBehavior>().displayName + "</b>";
        };

        playerOneDarts.OnDartSelected += setDart;
        playerTwoDarts.OnDartSelected += setDart;

        while (true)
        {
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
        dartDisplayName.text = "Game Over";
    }
}
