
using DG.Tweening;
using Qf.ClassDatas.AudioEdit;
using Qf.Events;
using Qf.Managers;
using QFramework;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 用于展示资源对象
/// </summary>
public class UIResourceItem : MonoBehaviour,IController,IPointerClickHandler
{
    [SerializeField]
    TMP_Text _Name;
    [SerializeField]
    Image _Image;
    AudioClip audioClip;
    void Start()
    {
        if (_Name != null)
            _Name = transform.GetChild(0).GetComponent<TMP_Text>();
        if (_Image != null)
            _Image = transform.GetChild(1).GetComponent<Image>();
    }
    public void SetAudioClip(AudioClip audioClip)
    {
        this.audioClip = audioClip;
    }
    public AudioClip GetAudioClip()
    {
        return audioClip;
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
        this.SendEvent(new SelectOptions() { SelectValue = audioClip,SelectObject = gameObject });
        AudioEditManager.Instance.OnePlay(audioClip);
    }
}