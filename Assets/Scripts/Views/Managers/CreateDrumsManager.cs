using Qf.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateDrumsManager:ManagerBase
{
    /// <summary>
    /// 添加鼓点实体
    /// </summary>
    /// <param name="operation"></param>
    /// <param name="vector3"></param>
    public void CreateDrums(TheTypeOfOperation operation,Vector3 vector3)
    {
        GameObject gameObject = Instantiate(Resources.Load<GameObject>(PathConfig.ProfabsOath + "InputMode"));
        InputMode mode = gameObject.GetComponent<InputMode>();
        mode.SetOperation(operation);
        gameObject.transform.position = vector3;
    }

    public override void Init()
    {
        Debug.Log("CreateDrumsManager 已加载...");
    }
}
