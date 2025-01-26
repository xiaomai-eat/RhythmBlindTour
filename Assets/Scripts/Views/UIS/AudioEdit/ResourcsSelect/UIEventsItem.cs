using DG.Tweening;
using Qf.Events;
using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEventsItem : MonoBehaviour, IController, IPointerClickHandler
{
    [SerializeField]
    TMP_Text _Name;
    [SerializeField]
    Image _Image;
    [SerializeField]
    UnityEvent clickeevent;
    public void AddAction(UnityAction unityAction)
    {
        clickeevent.AddListener(unityAction);
    }
    public void SetName(string Name)
    {
        _Name.text = Name;
    }
    /// <summary>
    /// 设置显示对象
    /// </summary>
    /// <param name="Sprite"></param>
    public void SetImage(Sprite Sprite)
    {
        _Image.sprite = Sprite;
    }
    public IArchitecture GetArchitecture()
    {
        return GameBody.Interface;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            transform.DOScale(new Vector3(1, 1, 1), 0.1f).SetEase(Ease.Linear);
        });
        clickeevent?.Invoke();
    }

    void Start()
    {
        if (_Name != null)
            _Name = transform.GetChild(0).GetComponent<TMP_Text>();
        if (_Image != null)
            _Image = transform.GetChild(1).GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
