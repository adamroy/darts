﻿using UnityEngine;
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

    public void Reset()
    {
        foreach(var db in buttons)
        {
            db.dartUsed = false;
            db.button.interactable = true;
            db.button.transform.localScale = Vector3.one;
        }

        var spin = playerTurnIndicator.GetComponent<SpinAndPop>();
        if (spin != null)
            Destroy(spin);
    }

    public Coroutine SignifyWin()
    {
        playerTurnIndicator.color = turnIndicatorColor;
        var spin = playerTurnIndicator.gameObject.AddComponent<SpinAndPop>();
        spin.spinTime = 0.25f;
        spin.spins = 6;
        spin.popScale = 1.5f;
        spin.popTime = 2f;
        return spin.Go();
    }
}
