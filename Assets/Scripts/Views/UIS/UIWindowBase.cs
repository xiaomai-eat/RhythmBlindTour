using QFramework;
using System;
using UnityEngine;
using UnityEngine.Events;

public class UIWindowBase : MonoBehaviour,IController
{
    UnityAction<UIWindowBase, Result> OnCloseHanlder;

    public virtual Type Type
    {
        get => GetType();
    }
    public enum Result
    {
        None=0,
        Yes,
        No
    }
    /// <summary>
    /// 每次启动UI的时候执行一次
    /// </summary>
    /// <param name="showData"></param>
    public virtual void OnShow(IUIData showData)
    {
    }
    /// <summary>
    /// 隐藏UI的时候执行一次
    /// </summary>
    public virtual void OnHide()
    {

    }
    /// <summary>
    /// 销毁UI的时候执行一次
    /// </summary>
    public virtual void OnDestroyClose()
    {
        
    }

    private void OnDestroy()
    {
        OnDestroyClose();
    }
    void Close(Result result,bool destroy=false)
    {
        OnCloseHanlder?.Invoke(this, result);
        OnHide();
        if (destroy)
            OnDestroyClose();
        UIManager.instance.Close(Type);
    }

    public virtual void HideYes()
    {
        Close(Result.Yes);
    }

    public virtual void HideNo()
    {
        Close(Result.No);
    }

    public virtual void CloseYes()
    {
        Close(Result.Yes,true);
    }

    public virtual void CloseNo()
    {
        Close(Result.Yes,true);
    }

    public IArchitecture GetArchitecture()
    {
        return GameBody.Interface;
    }
}
