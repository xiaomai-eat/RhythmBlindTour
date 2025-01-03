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

public class UITimeHand : MonoBehaviour,IController,IPointerClickHandler
{
    [SerializeField]
    RectTransform TimeHand;
    [SerializeField]
    TMP_Text ShowTime;
    private void Start()
    {
        this.RegisterEvent<OnUpdateThisTime>(v =>
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
        TimeHand.anchoredPosition = new Vector2(newTime *100,0);
        ShowTime.text = newTime.ToString("0.00");
    }

    public void AddTime()
    {

    }
    public void RemoveTime()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        TimeHand.position = eventData.position;
        this.SendCommand(new SetEditAudioThisTimeCommand((TimeHand.anchoredPosition.x) /100));
        this.SendEvent<OnUpdateThisTime>();
    }
    public IArchitecture GetArchitecture()
    {
        return GameBody.Interface;
    }

    
}
