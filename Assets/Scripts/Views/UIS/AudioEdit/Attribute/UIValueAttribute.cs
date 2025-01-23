using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIValueAttribute : UIAttributeBase, IPointerClickHandler
{
    [SerializeField]
    TMP_InputField inputField;
    private void Start()
    {
        inputField.onValueChanged.AddListener(v =>
        {
            RunAction(v);
        });
    }
    public void SetValueShow(string Value)
    {
        inputField.text = Value;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        SelectManager.SetAttribute(this);
    }
}
