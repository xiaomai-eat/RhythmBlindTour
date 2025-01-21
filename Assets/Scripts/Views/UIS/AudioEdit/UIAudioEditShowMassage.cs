using Qf.Commands.AudioEdit;
using Qf.Events;
using Qf.Models.AudioEdit;
using Qf.Querys.AudioEdit;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAudioEditShowMassage : MonoBehaviour, IController
{
    [SerializeField]
    TMP_Text _ModeShow;
    [SerializeField]
    TMP_Text _TimeShow;
    [SerializeField]
    TMP_Text _AudioNameShow;
    [SerializeField]
    TMP_InputField _InputField;
    public IArchitecture GetArchitecture()
    {
        return GameBody.Interface;
    }

    // Start is called before the first frame update
    void Start()
    {
        _InputField.onValueChanged.AddListener(v =>
        {
            if (v.Equals("") || _InputField.text.Equals(v)) return;
            this.SendCommand(new SetAudioEditAudioBPMCommand(int.Parse(v)));
        });
        this.RegisterEvent<OnEditMode>(v =>
        {
            _ModeShow.text = "编辑模式";
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnPlayMode>(v =>
        {
            _ModeShow.text = "游玩模式";
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        this.RegisterEvent<OnRecordingMode>(v =>
        {
            _ModeShow.text = "录制模式";
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnUpdateThisTime>(v =>
        {
            _TimeShow.text = v.ThisTime.ToString("0.00");
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<MainAudioChangeValue>(v =>
        {
            _AudioNameShow.text = v.Name;
           
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<BPMChangeValue>(v =>
        {
            _InputField.text = v.BPM.ToString();
            Debug.Log(v);
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
    }
    void Update()
    {

    }
}
