using Qf.Models.AudioEdit;
using Qf.Querys.AudioEdit;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

public class UIAudioEditBottonPlane : MonoBehaviour ,IController
{
    [SerializeField]
    RectTransform bottonPlane; //下侧面板
    [SerializeField]
    Image buttonImage; //按钮图片
    [SerializeField]
    RectTransform slideRegin; //滑动区域

    void Start()
    {
        Init();
    }
    void Init()
    {
        float SongTime = this.SendQuery(new QueryAudioEditAudioClipLength());
        slideRegin.sizeDelta = new Vector2(SongTime * 100, slideRegin.sizeDelta.y);
    }
    public void ToGglebottonPlaneShow()
    {
        if(bottonPlane.anchoredPosition == new Vector2(0, -400))
        {
            bottonPlane.anchoredPosition = new Vector2(0,0);
        }
        else
        {
            bottonPlane.anchoredPosition = new Vector2(0,-400);
        }
    }
    void Update()
    {
        
    }

    public IArchitecture GetArchitecture()
    {
        return GameBody.Interface;
    }
}
