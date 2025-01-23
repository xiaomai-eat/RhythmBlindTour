using Qf.Events;
using Qf.Models.AudioEdit;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : MonoBehaviour,IController
{
    private void OnEnable()
    {
        this.RegisterEvent<SelectOptions>(v =>
        {
            Debug.Log($"{((Object)v.SelectObject).name}");
        }).UnRegisterWhenDisabled(gameObject);
    }
    public void Save()
    {
        this.GetModel<AudioEditModel>().Save();
    }
    public void Load()
    {
        this.GetModel<AudioEditModel>().Load();
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
