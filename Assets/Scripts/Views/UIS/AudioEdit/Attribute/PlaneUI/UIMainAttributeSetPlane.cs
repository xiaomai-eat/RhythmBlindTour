
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
            editModel.TipOffset.Value = float.Parse((string)v);
        });
        editModel.TipOffset.Register(v =>
        {
            TipOffset.SetValueShow(v.ToString());
        }).UnRegisterWhenDisabled(gameObject);
        this.RegisterEvent<MainAudioChangeValue>(v =>
        {
            UpdateAll();
        }).UnRegisterWhenDisabled(gameObject);
    }
    void UpdateAll()
    {

        if (editModel.EditAudioClip != null)
            MainAudio.SetShowFileName(editModel.EditAudioClip.name);
        if (editModel.SucceedAudioClip != null)
            SucceedAudio.SetShowFileName(editModel.SucceedAudioClip.name);
        if (editModel.LoseAudioClip != null)
            LoseAudio.SetShowFileName(editModel.LoseAudioClip.name);
        TipOffset.SetValueShow(editModel.TipOffset.ToString());
    }
    void Update()
    {

    }
}
