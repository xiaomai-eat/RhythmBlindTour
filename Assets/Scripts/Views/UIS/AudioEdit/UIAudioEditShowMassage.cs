using Qf.Events;
using Qf.Querys.AudioEdit;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIAudioEditShowMassage : MonoBehaviour, IController
{
    [SerializeField]
    TMP_Text _ModeShow;
    [SerializeField]
    TMP_Text _TimeShow;
    [SerializeField]
    TMP_Text _AudioNameShow;
    public IArchitecture GetArchitecture()
    {
        return GameBody.Interface;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.RegisterEvent<OnEditMode>(v =>
        {
            _ModeShow.text = "EditMode";
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnPlayMode>(v =>
        {
            _ModeShow.text = "PlayMode";
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        this.RegisterEvent<OnRecordingMode>(v =>
        {
            _ModeShow.text = "RecordingMode";
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnUpdateThisTime>(v =>
        {
            _TimeShow.text = v.ThisTime.ToString("0.00");
        });
        this.RegisterEvent<MainAudioChangeValue>(v =>
        {
            _AudioNameShow.text = v.Name;
        });
    }
    void Update()
    {

    }
}
