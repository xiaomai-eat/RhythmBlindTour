using Qf.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateDrumsManager:ManagerBase
{
    public void Test1()
    {
        CreateDrums(TheTypeOfOperation.SwipeUp, new Vector3(0, 0, 0));
    }
    public void Test2()
    {
        CreateDrums(TheTypeOfOperation.SwipeDown, new Vector3(0, 0, 0));
    }
    public void Test3()
    {
        CreateDrums(TheTypeOfOperation.SwipeLeft, new Vector3(0, 0, 0));
    }
    public void Test4()
    {
        CreateDrums(TheTypeOfOperation.SwipeRight, new Vector3(0, 0, 0));
    }
    public void CreateDrums(TheTypeOfOperation operation,Vector3 vector3)
    {
        GameObject gameObject = Instantiate(Resources.Load<GameObject>(PathConfig.ProfabsOath + "InputMode"));
        InputMode mode = gameObject.GetComponent<InputMode>();
        mode.SetOperation(operation);
        gameObject.transform.position = vector3;
    }

    public override void Init()
    {
        Debug.Log("CreateDrumsManager ря╪сть...");
    }
}
