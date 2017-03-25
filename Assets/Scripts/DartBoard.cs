using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using Random = UnityEngine.Random;

public class DartBoard : MonoBehaviour
{
    public static DartBoard Instance { get; private set; }
    public Action<GameObject, int> OnPointsScored;
    public IEnumerable<GameObject> Darts { get { return dartInfo.Keys; } }
    public Transform[] spinners;
    public float slideHeight;

    private List<DartBoardSection> sections;
    private Dictionary<GameObject, DartInfo> dartInfo = new Dictionary<GameObject, DartInfo>();
    private class DartInfo
    {
        public GameObject dart;
        public DartBoardSection section;
        public int pointsScored;
        public int frameID;
    }

    private float rotationSpeed = 0;
    private Vector3 initialPosition;
    private float t = 0;
    private float slideSpeed = 0;
    private List<Coroutine> activeCoroutines;

    #region public interface methods

    // Determine if dart is a hit and attach accordingly
    public bool IsDartHit(GameObject dart, bool withAnimation)
    {
        foreach(var sec in sections)
        {
            if (sec.collider.OverlapPoint(dart.transform.position) && !sec.IsArmoured)
            {
                dart.transform.SetParent(sec.transform, true);
                dartInfo[dart] = new DartInfo { dart = dart, section = sec, frameID = Time.frameCount };
                if (withAnimation)
                    sec.Hit();
                return true;
            }
        }

        return false;
    }

    // Determine if the dart hit an armoured section
    public bool IsArmourSection(GameObject dart)
    {
        foreach (var sec in sections)
        {
            if (sec.collider.OverlapPoint(dart.transform.position)) 
            {
                return sec.IsArmoured;
            }
        }

        return false;
    }

    // Score the dart
    public void ScoreDart(GameObject dart, int multiplier = 1)
    {
        int points = dartInfo[dart].section.ScorePoints(dart.transform.position) * multiplier;
        dartInfo[dart].pointsScored = points;
        if (OnPointsScored != null) OnPointsScored(dart, points);
    }

    public void IncreaseRotation(float amount)
    {
        rotationSpeed += amount;
    }

    public void IncreaseSlide(float amount)
    {
        slideSpeed += amount;
    }

    public void ArmourSection(GameObject dart)
    {
        // Armour the section that this dart hit
        dartInfo[dart].section.IsArmoured = true;
    }

    public void KnockSection(GameObject dart)
    {
        var sectionToKnock = dartInfo[dart].section;
        KnockSection(sectionToKnock, dart);
    }

    public void RandomSectionKnock(GameObject dart)
    {
        var cr = StartCoroutine(RandomSectionKnockCoroutine(dart));
        activeCoroutines.Add(cr);
    }

    public void FinalizeScore()
    {
        foreach(var di in dartInfo.Values)
        {
            di.dart.SendMessage("Finish");
        }
    }

    public Coroutine AwaitActivities()
    {
        return StartCoroutine(AwaitActivitiesCoroutine());
    }

    public void Reset()
    {
        StopAllCoroutines();

        foreach(var sec in sections)
            sec.IsArmoured = false;

        foreach (var kvp in dartInfo)
            Destroy(kvp.Key);
        dartInfo.Clear();
    }

    #endregion

    #region Unity events

    private void Awake()
    {
        float degrees = 36f * Random.Range(0, 10);
        foreach (var t in spinners)
            t.Rotate(Vector3.forward, degrees, Space.World);
        Instance = this;
        sections = new List<DartBoardSection>(GetComponentsInChildren<DartBoardSection>());
        initialPosition = transform.position;
        activeCoroutines = new List<Coroutine>();
    }

    private void Update()
    {
        foreach (var transf in spinners)
            transf.Rotate(Vector3.forward, Time.deltaTime * rotationSpeed, Space.World);

        if (Mathf.Approximately(slideSpeed, 0) == false)
        {
            t += Time.deltaTime;
            transform.position = initialPosition + Mathf.Sin(t * slideSpeed) * slideHeight * Vector3.up;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, initialPosition, 1f * Time.deltaTime);
        }
    }

    #endregion
    
    #region private methods

    private void KnockSection(DartBoardSection sectionToKnock, GameObject dart)
    {
        var dartsRemoved = new List<GameObject>();

        // Knock other darts off the section this dart hit
        foreach (var currInfo in dartInfo.Values)
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

        sectionToKnock.IsArmoured = false;
    }

    private IEnumerator RandomSectionKnockCoroutine(GameObject dart)
    {
        var sections = this.sections.Where((s) => !s.isBullsEye).OrderBy(s => s.transform.localEulerAngles.z).ToList();
        var initialSec = dartInfo[dart].section;

        int index = sections.IndexOf(initialSec);
        if (index < 0) index = 0;
        float t = 0f;
        float v = Random.value * 0.1f;
        float a = 0.01f;

        while (true)
        {
            t -= Time.deltaTime;
            if(t <= 0)
            {
                index = (index + 1) % sections.Count;
                t = v;
                v += a;
                sections[index].ScaleUp(t, 1.2f);
                AudioManager.Play("boop");
                if (v > 0.4f)
                    break;
            }

            yield return null;
        }

        AudioManager.Play("dart_knock");
        KnockSection(sections[index], null);
    }

    private IEnumerator AwaitActivitiesCoroutine()
    {
        while(activeCoroutines.Count > 0)
        {
            var cr = activeCoroutines.First();
            yield return cr;
            activeCoroutines.Remove(cr);
        }
    }

    #endregion
}