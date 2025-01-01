using QFramework;
using System;
using System.Collections.Generic;

public class Managers : MonoSingleton<Managers>,IController
{
    public List<ManagerBase> ManagerOrderOfExecution = new();//¹ÜÀíÆ÷Ö´ÐÐË³Ðò
    Action _UpDate;
    Action _FixedUpDate;
    Action _LateUpDate;
    void Awake()
    {
        GetArchitecture();
        foreach (var manager in ManagerOrderOfExecution)
        {
            manager.Init();
        }
    }
    void Update() => _UpDate?.Invoke();

    void FixedUpdate() => _FixedUpDate?.Invoke();

    void LateUpdate() => _LateUpDate?.Invoke();

    public void AddUpdate(Action action) => _UpDate += action;
    public void AddFixedUpDate(Action action) => _FixedUpDate += action;
    public void AddLateUpDate(Action action) => _LateUpDate += action;

    public IArchitecture GetArchitecture()
    {
        return GameBody.Interface;
    }
}
