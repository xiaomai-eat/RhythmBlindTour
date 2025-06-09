using Qf.ClassDatas.AudioEdit;
using Qf.Commands.AudioEdit;
using Qf.Events;
using Qf.Models;
using Qf.Models.AudioEdit;
using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class UIAttributeSetPanel : MonoBehaviour, IController
{
    AudioEditModel editModel;
    DataCachingModel cModel;
    [SerializeField]
    TMP_Text Name;
    [SerializeField]
    Button UpButton;
    [SerializeField]
    Button DownButton;
    [SerializeField]
    UIDorpAttribute DrwmType;
    [SerializeField]
    UIFileAttribute PreAdventAudio;
    [SerializeField]
    UIFileAttribute SucceedAudio;
    [SerializeField]
    UIFileAttribute LoseAudioClip;
    [SerializeField]
    UISliderAttribute PreAdventAudioVolum;
    [SerializeField]
    UISliderAttribute SucceedAudioVolum;
    [SerializeField]
    UISliderAttribute LoseAudioClipVolum;
    [SerializeField]
    UIValueAttribute TimeOfExistence; //存在时间
    [SerializeField]
    UIValueAttribute PreAdventAudioClipOffsetTime;//偏移
    [SerializeField]
    Button RemoveButton;
    public IArchitecture GetArchitecture()
    {
        return GameBody.Interface;
    }
    int index;
    private void OnEnable()
    {
        if (editModel == null)
            editModel = this.GetModel<AudioEditModel>();
        if (cModel == null)
            cModel = this.GetModel<DataCachingModel>();
        this.RegisterEvent<OnUpdateThisTime>(v =>
        {
            if (editModel.Mode.Equals(SystemModeData.PlayMode)) return;
            index = 0;
            if (editModel.TimeLineData.ContainsKey(v.ThisTime))
            {
                UpButton.gameObject.SetActive(false);
                DownButton.gameObject.SetActive(true);
            }
            else
            {
                UpButton.gameObject.SetActive(false);
                DownButton.gameObject.SetActive(false);
                thisTime = (float)Math.Round(v.ThisTime, 2, MidpointRounding.ToEven);
                UpdateName();
                Show(false);
                return;
            }
            UpdateData(index);
        }).UnRegisterWhenDisabled(gameObject);
        this.RegisterEvent<OnUpdateAudioEditDrumsUI>(v =>
        {
            if (editModel.Mode.Equals(SystemModeData.PlayMode)) return;
            if (editModel.TimeLineData.ContainsKey(editModel.ThisTime))
            {
                if (editModel.TimeLineData[editModel.ThisTime].Count <= index)
                {
                    index = editModel.TimeLineData[editModel.ThisTime].Count - 1;
                }
                if (editModel.TimeLineData[editModel.ThisTime].Count != 0)
                {
                    if (index == 0)
                    {
                        UpButton.gameObject.SetActive(false);
                        DownButton.gameObject.SetActive(true);
                    }
                    else if (index == editModel.TimeLineData[editModel.ThisTime].Count - 1)
                    {
                        UpButton.gameObject.SetActive(true);
                        DownButton.gameObject.SetActive(false);
                    }
                    else
                    {
                        UpButton.gameObject.SetActive(true);
                        DownButton.gameObject.SetActive(true);
                    }
                    UpdateData(index);
                }
                else
                {
                    UpdateName("当前时间节点无鼓点");
                    UpButton.gameObject.SetActive(false);
                    DownButton.gameObject.SetActive(false);
                    Show(false);
                }

            }
            else
            {
                UpButton.gameObject.SetActive(false);
                DownButton.gameObject.SetActive(false);
                thisTime = (float)Math.Round(editModel.ThisTime, 2, MidpointRounding.ToEven);
                UpdateName();
                Show(false);
            }
        }).UnRegisterWhenDisabled(gameObject);
        index = 0;
        if (editModel.TimeLineData.ContainsKey(editModel.ThisTime))
        {
            UpButton.gameObject.SetActive(false);
            DownButton.gameObject.SetActive(true);
            Show(true);
        }
        else
        {
            UpButton.gameObject.SetActive(false);
            DownButton.gameObject.SetActive(false);
            Show(false);
            return;
        }
        UpdateData(index);
    }
    List<GameObject> gameObjects = new();
    void Start()
    {
        UpButton.gameObject.SetActive(false);
        DownButton.gameObject.SetActive(false);
        if (editModel == null)
            editModel = this.GetModel<AudioEditModel>();
        if (cModel == null)
            cModel = this.GetModel<DataCachingModel>();
        gameObjects.Add(DrwmType.transform.parent.gameObject);
        gameObjects.Add(PreAdventAudio.transform.parent.gameObject);
        gameObjects.Add(SucceedAudio.transform.parent.gameObject);
        gameObjects.Add(LoseAudioClip.transform.parent.gameObject);
        gameObjects.Add(PreAdventAudioVolum.transform.parent.gameObject);
        gameObjects.Add(SucceedAudioVolum.transform.parent.gameObject);
        gameObjects.Add(LoseAudioClipVolum.transform.parent.gameObject);
        gameObjects.Add(TimeOfExistence.transform.parent.gameObject);
        gameObjects.Add(PreAdventAudioClipOffsetTime.transform.parent.gameObject);
    }
    float thisTime;
    DrumsLoadData ls;
    void UpdateData(int index = 0)
    {
        thisTime = (float)Math.Round(editModel.ThisTime, 2, MidpointRounding.ToEven);
        UpdateName();
        if (editModel.TimeLineData.ContainsKey(thisTime))
        {
            ls = editModel.TimeLineData[thisTime][index];
            UpdateDataShow(ls);
            DrwmType.SetAction(v =>
            {
                ls.DrwmsData.DtheTypeOfOperation = (TheTypeOfOperation)v;
                DrwmType.SetDropdownVlaue(ls.DrwmsData.DtheTypeOfOperation);
                this.SendEvent<OnUpdateAudioEditDrumsUI>();

            });
            PreAdventAudio.SetAction(v =>
            {
                ls.DrwmsData.FPreAdventAudioClipPath = ((AudioClip)v).name;
                PreAdventAudio.SetShowFileName(ls.DrwmsData.FPreAdventAudioClipPath);
                this.SendEvent<OnUpdateAudioEditDrumsUI>();
            });
            SucceedAudio.SetAction(v =>
            {
                ls.DrwmsData.FSucceedAudioClipPath = ((AudioClip)v).name;
                SucceedAudio.SetShowFileName(ls.DrwmsData.FSucceedAudioClipPath);
                this.SendEvent<OnUpdateAudioEditDrumsUI>();
            });
            LoseAudioClip.SetAction(v =>
            {
                ls.DrwmsData.FLoseAudioClipPath = ((AudioClip)v).name;
                LoseAudioClip.SetShowFileName(ls.DrwmsData.FLoseAudioClipPath);
                this.SendEvent<OnUpdateAudioEditDrumsUI>();

            });
            PreAdventAudioVolum.SetAction(v =>
            {
                ls.MusicData.SPreAdventVolume = (float)v;
                PreAdventAudioVolum.SetValueShow(ls.MusicData.SPreAdventVolume);
                this.SendEvent<OnUpdateAudioEditDrumsUI>();

            });
            SucceedAudioVolum.SetAction(v =>
            {
                ls.MusicData.SSucceedVolume = (float)v;
                SucceedAudioVolum.SetValueShow(ls.MusicData.SSucceedVolume);
                this.SendEvent<OnUpdateAudioEditDrumsUI>();

            });
            LoseAudioClipVolum.SetAction(v =>
            {
                ls.MusicData.SLoseVolume = (float)v;
                LoseAudioClipVolum.SetValueShow(ls.MusicData.SLoseVolume);
                this.SendEvent<OnUpdateAudioEditDrumsUI>();

            });
            TimeOfExistence.SetAction(v =>
            {
                ls.DrwmsData.VTimeOfExistence = float.Parse((string)v);
                TimeOfExistence.SetValueShow(ls.DrwmsData.VTimeOfExistence.ToString());
                this.SendEvent<OnUpdateAudioEditDrumsUI>();
            });
            PreAdventAudioClipOffsetTime.SetAction(v =>
            {
                ls.DrwmsData.VPreAdventAudioClipOffsetTime = float.Parse((string)v);
                PreAdventAudioClipOffsetTime.SetValueShow(ls.DrwmsData.VPreAdventAudioClipOffsetTime.ToString());
                this.SendEvent<OnUpdateAudioEditDrumsUI>();
            });
            Show(true);
        }
        else
        {
            Show(false);
        }
    }
    void UpdateDataShow(DrumsLoadData ls)
    {
        DrwmType.SetDropdownVlaue(ls.DrwmsData.DtheTypeOfOperation);
        PreAdventAudio.SetShowFileName(ls.DrwmsData.FPreAdventAudioClipPath);
        SucceedAudio.SetShowFileName(ls.DrwmsData.FSucceedAudioClipPath);
        LoseAudioClip.SetShowFileName(ls.DrwmsData.FLoseAudioClipPath);
        PreAdventAudioVolum.SetValueShow(ls.MusicData.SPreAdventVolume);
        SucceedAudioVolum.SetValueShow(ls.MusicData.SSucceedVolume);
        LoseAudioClipVolum.SetValueShow(ls.MusicData.SLoseVolume);
        TimeOfExistence.SetValueShow(ls.DrwmsData.VTimeOfExistence.ToString());
        PreAdventAudioClipOffsetTime.SetValueShow(ls.DrwmsData.VPreAdventAudioClipOffsetTime.ToString());
    }
    public void Show(bool isbool)
    {
        foreach (var i in gameObjects)
        {
            i.SetActive(isbool);
        }
        RemoveButton.gameObject.SetActive(isbool);
    }
    public void RemoveDrwm()
    {
        thisTime = (float)Math.Round(editModel.ThisTime, 2, MidpointRounding.ToEven);
        this.SendCommand(new RemoveAudioEditTimeLineDataCommand(thisTime, index));
    }
    public void UpdateName(string str = "")
    {
        if (!str.Equals(""))
        {
            Name.text = str;
            return;
        }
        if (editModel.TimeLineData.ContainsKey(thisTime))
        {
            if (editModel.TimeLineData[thisTime].Count >= index) return;
            Name.text = editModel.TimeLineData[thisTime][index].Name;
        }
        else
        {
            Name.text = "当前时间节点无鼓点";
        }
    }
    public void UpQh()
    {
        index--;
        if (index <= 0)
        {
            index = 0;
            UpButton.gameObject.SetActive(false);
        }
        else
        {
            UpButton.gameObject.SetActive(true);
        }
        DownButton.gameObject.SetActive(true);
        UpdateData(index);
    }
    public void DownQh()
    {
        index++;
        if (editModel.TimeLineData.ContainsKey(thisTime))
        {
            if (index >= editModel.TimeLineData[thisTime].Count - 1)
            {
                index = editModel.TimeLineData[thisTime].Count - 1;
                DownButton.gameObject.SetActive(false);
            }
            else
            {
                DownButton.gameObject.SetActive(true);
            }
            UpButton.gameObject.SetActive(true);
        }
        UpdateData(index);
    }
    void Update()
    {

    }
}