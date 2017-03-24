using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;

public class DartSelector : MonoBehaviour
{
    public Action<GameObject> OnDartSelected;
    public GameObject NextDart { get { var dbtn = buttons.FirstOrDefault((db) => db.dartUsed == false); return dbtn == null ? null : dbtn.dart; } }

    [Serializable]
    public class DartButton
    {
        [HideInInspector]
        public bool dartUsed;
        public GameObject dart;
        public Button button;
    }

    [Range(0f, 1f)]
    public float usedIconScale;
    public Image playerTurnIndicator;
    public List<DartButton> buttons;

    private Color turnIndicatorColor;

    private void Start()
    {
        turnIndicatorColor = playerTurnIndicator.color;
        buttons = buttons.OrderBy(b => -b.button.transform.position.y).ToList();
    }

    public void SetEnabled(bool enabled)
    {
        playerTurnIndicator.color = enabled ? turnIndicatorColor : buttons[0].button.colors.disabledColor;

        foreach (var db in buttons)
        {
            if (!db.dartUsed)
            {
                db.button.interactable = enabled;
            }
        }
    }

    public void ButtonPresseed(Button btn)
    {
        var dartButton = buttons.First((b) => b.button == btn);
        if (OnDartSelected != null)
            OnDartSelected(dartButton.dart);
    }

    public void DartUsed(GameObject dart)
    {
        var dartButton = buttons.First((b) => b.dart == dart && b.dartUsed == false);
        dartButton.button.transform.localScale = Vector3.one * usedIconScale;
        dartButton.button.interactable = false;
        dartButton.dartUsed = true;
    }

    public void SetSelectedDart(GameObject dart)
    {
        var dartButton = buttons.First((b) => b.dart == dart);
        dartButton.button.Select();
    }
}
