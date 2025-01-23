using Qf.Events;
using Qf.Systems;
using QFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour,IController
{
  

    public IArchitecture GetArchitecture()
    {
        return GameBody.Interface;
    }
}