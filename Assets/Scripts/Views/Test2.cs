using Qf.Events;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : MonoBehaviour,IController
{
    private void OnEnable()
    {
        this.RegisterEvent<SelectAudio>(v =>
        {
            Debug.Log($"{v.SelectAudioClip.name}");
        }).UnRegisterWhenDisabled(gameObject);
    }

    public IArchitecture GetArchitecture()
    {
        return GameBody.Interface;
    }
    public void Run()
    {
        this.SendEvent<TestEvent>();
    }
}
