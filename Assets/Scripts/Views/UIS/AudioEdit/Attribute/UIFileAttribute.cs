using Qf.Commands.AudioEdit;
using Qf.Models.AudioEdit;
using QFramework;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIFileAttribute : UIAttributeBase, IPointerClickHandler
{
    [SerializeField]
    TMP_Text ShowFileName;
    private void Start()
    {
        SetParameterType(ParameterType.File);
    }
    public void SetShowFileName(string Name)
    {
        ShowFileName.text = Name;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        SelectManager.SetAttribute(this);
    }
}
