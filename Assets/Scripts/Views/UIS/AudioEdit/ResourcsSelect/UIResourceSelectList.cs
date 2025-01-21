using Qf.Models;
using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIResourceSelectList : MonoBehaviour, IController
{
    [SerializeField]
    GameObject UIItemProfab;
    [SerializeField]
    GameObject ObjectParent;
    List<UIResourceItem> _UIItems = new();
    List<AudioClip> audioClips;
    private void Start()
    {
        audioClips = this.GetModel<DataCachingModel>().ResourceAudioDatas;
    }
    UIResourceItem resourceItem;
    public void UpDateList()
    {
        if (_UIItems.Count >= audioClips.Count)
        {
            for (int i = 0; i < _UIItems.Count; i++)
            {
                if (i < audioClips.Count)
                {
                    _UIItems[i].gameObject.SetActive(true);
                    _UIItems[i].SetAudioClip(audioClips[i]);
                    _UIItems[i].SetName(audioClips[i].name);
                    continue;
                }
                _UIItems[i].gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < audioClips.Count; i++)
            {
                if (i >= _UIItems.Count)
                {
                    resourceItem = Instantiate(UIItemProfab).GetComponent<UIResourceItem>();
                    resourceItem.transform.SetParent(ObjectParent.transform);
                    _UIItems.Add(resourceItem);
                    resourceItem.SetAudioClip(audioClips[i]);
                    resourceItem.SetName(audioClips[i].name);
                    continue;
                }
                _UIItems[i].gameObject.SetActive(true);
                _UIItems[i].SetAudioClip(audioClips[i]);
                _UIItems[i].SetName(audioClips[i].name);
            }
        }

    }

    public IArchitecture GetArchitecture()
    {
        return GameBody.Interface;
    }
}