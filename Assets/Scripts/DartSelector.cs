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
    public List<DartButton> buttons;

    public void SetEnabled(bool enabled)
    {
        foreach(var db in buttons)
        {
            if (!db.dartUsed)
                db.button.interactable = enabled;
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
        var dartButton = buttons.First((b) => b.dart == dart);
        dartButton.button.interactable = false;
        dartButton.dartUsed = true;
    }
}
