using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
public class UIDorpAttribute : UIAttributeBase, IPointerClickHandler
{
    [SerializeField]
    TMP_Dropdown Dropdown;
    TheTypeOfOperation TheTypeOfOperation;
    private void Start()
    {
        SetParameterType(ParameterType.Drop);
    }
    public void SetDropdownVlaue<T>(T Value) where T:Enum//用于不同枚举的显示
    {
    }
    public void SetDropdownVlaue(TheTypeOfOperation theTypeOfOperation)//默认
    {
        TheTypeOfOperation = theTypeOfOperation;
    }
    public TheTypeOfOperation GetDropdownVlaue()//默认
    {
        return TheTypeOfOperation;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        SelectManager.SetAttribute(this);
    }
}