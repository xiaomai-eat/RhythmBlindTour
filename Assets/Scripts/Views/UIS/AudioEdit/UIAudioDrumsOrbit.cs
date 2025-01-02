using Qf.Models.AudioEdit;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudioDrumsOrbit : MonoBehaviour,IController
{
    [SerializeField]
    RectTransform[] DrumsUI;
    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return GameBody.Interface;
    }
    void Init()
    {
        float SongTime = this.SendQuery(new QueryAudioEditAudioClipLength());
        for (int i = 0; i < DrumsUI.Length; i++)
        {
            DrumsUI[i].sizeDelta = new Vector2(SongTime*150-120,100);
        }
    }
    void Start()
    {
        Init();
    }

}
