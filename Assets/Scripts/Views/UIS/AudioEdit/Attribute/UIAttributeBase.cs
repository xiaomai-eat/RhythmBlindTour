using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIAttributeBase : MonoBehaviour,IController
{
    [SerializeField]
    TMP_Text _Name;//属性名称
    [SerializeField]
    ParameterType _ParameterType;//参数类型
    Action<object> action;
    public void SetAction(Action<object> action)
    {
        this.action = action;
    }
    public void RunAction(object value)
    {
        action?.Invoke(value);
    }
    public void SetName(string Name)
    {
        _Name.text = Name;
    }
    public ParameterType GetParameterType()
    {
        return _ParameterType;
    }

    public IArchitecture GetArchitecture()
    {
        return GameBody.Interface;
    }
}
/// <summary>
/// 参数类型
/// </summary>
public enum ParameterType
{
    Value,
    File,
}
