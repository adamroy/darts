using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class DartBoard : MonoBehaviour
{
    public static DartBoard Instance { get; private set; }
    public Action<GameObject, int> OnPointsScored;
    public IEnumerable<GameObject> Darts { get { return dartInfo.Keys; } }
    public Transform[] spinners;

    private List<DartBoardSection> sections;
    private Dictionary<GameObject, DartInfo> dartInfo = new Dictionary<GameObject, DartInfo>();
    private class DartInfo
    {
        public GameObject dart;
        public DartBoardSection section;
        public int pointsScored;
    }

    private float rotationSpeed = 0;

    #region public interface methods

    public bool IsDartHit(GameObject dart)
    {
        // Determine if dart is a hit and attach accordingly
        foreach(var sec in sections)
        {
            if (sec.collider.OverlapPoint(dart.transform.position) && !sec.IsArmoured)
            {
                dart.transform.SetParent(sec.transform, true);
                dartInfo[dart] = new DartInfo { dart = dart, section = sec };
                sec.Hit();
                return true;
            }
        }

        return false;
    }

    public bool IsArmourSection(GameObject dart)
    {
        // Determine if the dart hit an armoured section
        foreach (var sec in sections)
        {
            if (sec.collider.OverlapPoint(dart.transform.position)) 
            {
                return sec.IsArmoured;
            }
        }

        return false;
    }

    public void ScoreDart(GameObject dart, int multiplier = 1)
    {
        // Score the dart
        int points = dartInfo[dart].section.ScorePoints(dart.transform.position) * multiplier;
        dartInfo[dart].pointsScored = points;

        print(points);
        if (OnPointsScored != null) OnPointsScored(dart, points);
    }

    public void IncreaseRotation(float amount)
    {
        rotationSpeed += amount;
    }

    public void ArmourSection(GameObject dart)
    {
        // Armour the section that this dart hit
        dartInfo[dart].section.IsArmoured = true;
    }

    public void KnockSection(GameObject dart)
    {
        var sectionToKnock = dartInfo[dart].section;
        var dartsRemoved = new List<GameObject>();

        // Knock other darts off the section this dart hit
        foreach(var currInfo in dartInfo.Values)
        {
            if (currInfo.section == sectionToKnock && currInfo.dart != dart)
            {
                // Lose the points for each dart
                if (OnPointsScored != null)
                    OnPointsScored(currInfo.dart, -currInfo.pointsScored);

                // Make dart fall off the board
                currInfo.dart.SendMessage("Fall");

                // Keep track of which darts to remove after iteration is complete
                dartsRemoved.Add(currInfo.dart);
            }
        }

        // Actually remove the dart
        foreach (var d in dartsRemoved)
            dartInfo.Remove(d);
    }

    #endregion

    private void Awake()
    {
        float degrees = 36f * Random.Range(0, 10);
        foreach (var t in spinners)
            t.Rotate(Vector3.forward, degrees, Space.World);
        Instance = this;
        sections = new List<DartBoardSection>(GetComponentsInChildren<DartBoardSection>());
    }

    private void Update()
    {
        foreach (var t in spinners)
            t.Rotate(Vector3.forward, Time.deltaTime * rotationSpeed, Space.World);
    }
}