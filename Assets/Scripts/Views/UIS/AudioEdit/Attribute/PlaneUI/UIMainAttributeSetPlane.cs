
using Assets.Scripts.Querys.AudioEdit;
using Qf.Commands.AudioEdit;
using Qf.Events;
using Qf.Models.AudioEdit;
using QFramework;

using UnityEngine;

public class UIMainAttributeSetPlane : MonoBehaviour, IController
{
    [SerializeField]
    UIFileAttribute MainAudio;//主音频
    [SerializeField]
    UIFileAttribute SucceedAudio;//成功音频
    [SerializeField]
    UIFileAttribute LoseAudio;//失败音频
    [SerializeField]
    UIValueAttribute TipOffset;//偏移量
    [SerializeField]
    UIValueAttribute TimeOfExistence;//鼓点存在时间
    [SerializeField]
    UIFileAttribute[] InteractionEvents;
    AudioEditModel editModel;
    private void OnEnable()
    {
        if (editModel == null)
            editModel = this.GetModel<AudioEditModel>();
        UpdateAll();
    }
    public IArchitecture GetArchitecture()
    {
        return GameBody.Interface;
    }

    void Start()
    {
        MainAudio.SetAction(v =>
        {
            this.SendCommand(new SetAudioEditAudioCommand((AudioClip)v));
            MainAudio.SetShowFileName(((AudioClip)v).name);
        });
        SucceedAudio.SetAction(v =>
        {
            this.SendCommand(new SetAudioEditSucceedAudioCommand((AudioClip)v));
            SucceedAudio.SetShowFileName(((AudioClip)v).name);
        });
        LoseAudio.SetAction(v =>
        {
            this.SendCommand(new SetAudioEditAudioLoseAudioCommand((AudioClip)v));
            LoseAudio.SetShowFileName(((AudioClip)v).name);
        });
        TipOffset.SetAction(v =>
        {
            if (!v.Equals(""))
                editModel.TipOffset.Value = float.Parse((string)v);
            else
                editModel.TipOffset.Value = 0;
        });
        TimeOfExistence.SetAction(v =>
        {
            if (!v.Equals(""))
                editModel.TimeOfExistence.Value = float.Parse((string)v);
            else
                editModel.TipOffset.Value = 0;
        });
        editModel.TipOffset.Register(v =>
        {
            TipOffset.SetValueShow(v.ToString());
        }).UnRegisterWhenDisabled(gameObject);
        editModel.TimeOfExistence.Register(v =>
        {
            TimeOfExistence.SetValueShow(v.ToString());
        }).UnRegisterWhenDisabled(gameObject);
        int index = 0;
        foreach (var i in InteractionEvents)//上下左右单击
        {
            int ls = index;
            i.SetAction(v =>
            {
                
                this.SendCommand(new SetAudioEditAudioComeTipsCommand((TheTypeOfOperation)ls, (AudioClip)v));
                i.SetShowFileName(((AudioClip)v).name);
            });
            index++;
        }
        this.RegisterEvent<OnUpdateAudioEditDrumsUI>(v =>
        {
            UpdateAll();
        }).UnRegisterWhenDisabled(gameObject);

    }
    void UpdateAll()
    {
        MainAudio.SetShowFileName(editModel?.EditAudioClip?.name);
        SucceedAudio.SetShowFileName(editModel?.SucceedAudioClip?.name);
        LoseAudio.SetShowFileName(editModel?.LoseAudioClip?.name);
        int index = 0;
        foreach (var i in InteractionEvents)
        {
            i.SetShowFileName(this.SendQuery(new QueryAudioEditComeTipAudio((TheTypeOfOperation)index))?.name);
            index++;
        }
        TipOffset.SetValueShow(editModel.TipOffset.ToString());
        TimeOfExistence.SetValueShow(editModel.TimeOfExistence.ToString());
    }
    public void UpdateAllData()
    {
        foreach (var i in editModel.TimeLineData.Keys)
        {
            foreach (var j in editModel.TimeLineData[i])
            {
                j.DrwmsData.SucceedAudioClipPath = editModel.SucceedAudioClip.name;
                j.DrwmsData.LoseAudioClipPath = editModel.LoseAudioClip.name;
                j.DrwmsData.PreAdventAudioClipPath = this.SendQuery(new QueryAudioEditComeTipAudio(j.DrwmsData.theTypeOfOperation)).name;
                j.DrwmsData.PreAdventAudioClipOffsetTime = editModel.TipOffset.Value;
                j.DrwmsData.TimeOfExistence = editModel.TimeOfExistence.Value;
            }
        }
        this.SendEvent<OnUpdateAudioEditDrumsUI>();
    }
    void Update()
    {

    }
}
