using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAudioEditTimeDataShowButton : MonoBehaviour, IController
{
    [SerializeField]
    TMP_Text _Timetext;
    [SerializeField]
    float _Keys;
    public void SetShowText(string str)
    {
        _Timetext.text = str;
    }
    public void SetShowPlane(float Keys)
    {
        _Keys = Keys;
    }
    public void ShowPlane()
    {
        Debug.Log("[测试] 显示面板");
    }
    public IArchitecture GetArchitecture()
    {
        return GameBody.Interface;
    }
}