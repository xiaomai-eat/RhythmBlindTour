using Qf.Events;
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
    int _PixelUnitsPerSecond = AudioEditConfig.PixelUnitsPerSecond;//每秒像素单位
    int _EditHeight = AudioEditConfig.EditHeight;//编辑器可编辑范围高度
    void Start()
    {
        Init();
        this.RegisterEvent<MainAudioChangeValue>(v=>Init()).UnRegisterWhenGameObjectDestroyed(gameObject);
    }
    void Init()
    {
        float SongTime = this.SendQuery(new QueryAudioEditAudioClipLength());
        slideRegin.sizeDelta = new Vector2(SongTime * _PixelUnitsPerSecond, slideRegin.sizeDelta.y);
    }
    public void ToGglebottonPlaneShow()
    {
        if(bottonPlane.anchoredPosition == new Vector2(0, -_EditHeight))
        {
            bottonPlane.anchoredPosition = new Vector2(0,0);
        }
        else
        {
            bottonPlane.anchoredPosition = new Vector2(0,-_EditHeight);
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
