using Qf.Models.AudioEdit;
using Qf.Querys.AudioEdit;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudioDrumsOrbit : MonoBehaviour,IController
{
    [SerializeField]
    GameObject DrumsProfabs;//鼓点预制体
    [SerializeField]
    RectTransform[] DrumsUI;
    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return GameBody.Interface;
    }
    public void AddDrwms()
    {
        Debug.Log("生成鼓点");
        //判断当前位置是否已经存在鼓点
        //生成鼓点
        //设置鼓点位置
    }
    public void RemoveDrwms()
    {
        Debug.Log("删除鼓点");
        //判断当前位置是否已经存在鼓点
        //当前鼓点
        //清除鼓点
    }
    void Init()
    {
        //初始化轨道长度
        float SongTime = this.SendQuery(new QueryAudioEditAudioClipLength());
        for (int i = 0; i < DrumsUI.Length; i++)
        {
            DrumsUI[i].sizeDelta = new Vector2(SongTime*100,80);
        }

        //初始话鼓点数据
        //没写
    }
    void Start()
    {
        Init();
    }

}
