using Qf.Commands.AudioEdit;
using Qf.Events;
using Qf.Querys.AudioEdit;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIAudioEditTimeHand : MonoBehaviour, IController, IPointerClickHandler
{
    [SerializeField]
    RectTransform TimeHand;
    [SerializeField]
    TMP_Text ShowTimes;
    [SerializeField]
    ScrollRect ScrollRect;
    int _PixelUnitsPerSecond = AudioEditConfig.PixelUnitsPerSecond;//ÿ�����ص�λ
    int _EditHeight = AudioEditConfig.EditHeight;//�༭���ɱ༭��Χ�߶�
    public Vector2 TimeHandPos
    {
        get
        {
            return TimeHand.anchoredPosition;
        }
        set
        {
            TimeHand.anchoredPosition = value;
        }
    }
    private void Start()
    {
        this.RegisterEvent<OnUpdateThisTime>(v =>
        {
            TimeHand.anchoredPosition = new Vector2(v.ThisTime * _PixelUnitsPerSecond, 0);
            ShowTimes.text = v.ThisTime.ToString("0.00");
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<MainAudioChangeValue>(v =>
        {
            UpdateThisTime();
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
    }
    private void OnEnable()
    {
        this.SendEvent<OnUpdateThisTime>();
    }
    void UpdateThisTime()
    {
        float newTime = this.SendQuery(new QueryAudioEditAudioClipThisTime());
        TimeHand.anchoredPosition = new Vector2(newTime * _PixelUnitsPerSecond, 0);
        ShowTimes.text = newTime.ToString("0.00");
    }
    int mode = 0;
    float Speed;
    float PressTime;
    private void Update()
    {
        #region ģʽ�л�
        if (mode == 0)
        {
            PressTime = 0;
        }
        else if (mode == 1)
        {
            if (PressTime >= 0.5f)
            {
                AddTime(Speed);
            }
            else
            {
                PressTime += Time.deltaTime;
            }
        }
        else if (mode == 2)
        {
            if (PressTime >= 0.5f)
            {
                RemoveTime(Speed);
            }
            else
            {
                PressTime += Time.deltaTime;
            }
        }
        #endregion
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            RemoveTime(1);
            RemoveTimeMode(1);
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            StopTimeMode(0);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            AddTime(1);
            AddTimeMode(1);
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            StopTimeMode(0);
        }
    }
    public void AddTimeMode(float Speed)
    {
        mode = 1;
        this.Speed = Speed;
    }
    public void StopTimeMode(float Speed)
    {
        mode = 0;
        this.Speed = Speed;
    }
    public void RemoveTimeMode(float Speed)
    {
        mode = 2;
        this.Speed = Speed;
    }
    public void SetTime(float Time)
    {
        this.SendCommand(new SetAudioEditThisTimeCommand(Time));
    }
    public void AddTime(float Speed)
    {
        this.SendCommand(
            new SetAudioEditThisTimeCommand(
                this.SendQuery(
                    new QueryAudioEditAudioClipThisTime()) + 0.01f * Speed));
        ScrollRect.horizontalScrollbar.value = (TimeHand.anchoredPosition.x / _PixelUnitsPerSecond) / this.SendQuery(new QueryAudioEditAudioClipLength());
    }
    public void RemoveTime(float Speed)
    {
        this.SendCommand(
            new SetAudioEditThisTimeCommand(
                this.SendQuery(
                    new QueryAudioEditAudioClipThisTime()) - 0.01f * Speed));
        ScrollRect.horizontalScrollbar.value = (TimeHand.anchoredPosition.x / _PixelUnitsPerSecond) / this.SendQuery(new QueryAudioEditAudioClipLength());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        FindObjectOfType<CreateDrumsManager>()?.ResetAllActiveCenters(); // 清空标记 局限在谱面编辑中可以重新设置InputMode //2025/06/10 - mixyao
        TimeHand.position = eventData.position;
        this.SendCommand(new SetAudioEditThisTimeCommand((TimeHand.anchoredPosition.x) / _PixelUnitsPerSecond));
    }
    public IArchitecture GetArchitecture()
    {
        return GameBody.Interface;
    }


}
