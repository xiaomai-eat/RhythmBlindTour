using Qf.Models.AudioEdit;
using Qf.Querys.AudioEdit;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudioDrumsOrbit : MonoBehaviour,IController
{
    [SerializeField]
    RectTransform[] DrumsUI;
    [SerializeField]
    GameObject DrumsProfabs;
    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return GameBody.Interface;
    }
    public void AddDrwms()
    {
        //生成鼓点
        //设置鼓点位置
    }
    public void RemoveDrwms()
    {
        //当前鼓点
        //清除鼓点
    }
    void Init()
    {
        float SongTime = this.SendQuery(new QueryAudioEditAudioClipLength());
        for (int i = 0; i < DrumsUI.Length; i++)
        {
            DrumsUI[i].sizeDelta = new Vector2(SongTime*100,80);
        }
    }
    void Start()
    {
        Init();
    }

}
