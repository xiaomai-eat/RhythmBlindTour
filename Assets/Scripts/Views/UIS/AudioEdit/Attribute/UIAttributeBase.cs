using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIAttributeBase : MonoBehaviour
{
    [SerializeField]
    TMP_Text _Name;//属性名称
    [SerializeField]
    ParameterType _ParameterType;//参数类型
    public void SetName(string Name)
    {
        _Name.text = Name;
    }
    public ParameterType GetParameterType()
    {
        return _ParameterType;
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
