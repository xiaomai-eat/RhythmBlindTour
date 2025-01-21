using DG.Tweening;
using Qf.Commands.AudioEdit;
using Qf.Events;
using Qf.Managers;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectManager : ManagerBase
{
    static UIAttributeBase LastAttribute;
    static UIAttributeBase CurrentAttribute;
    public override void Init()
    {
        this.RegisterEvent<SelectAudio>(v =>
        {
            if (CurrentAttribute == null) return;
            if (CurrentAttribute.GetParameterType().Equals(ParameterType.File)){
                //Debug.Log("Ö´ÐÐ");
                if (CurrentAttribute != null)
                    this.SendCommand(new SetAudioEditAudioCommand(v.SelectAudioClip));
                UIFileAttributte ls = (UIFileAttributte)CurrentAttribute;
                ls.SetShowFileName(v.SelectAudioClip.name);
            }
            
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
    }
    public static void SetAttribute(UIAttributeBase uIAttributeBase)
    {
        LastAttribute = CurrentAttribute;
        CurrentAttribute = uIAttributeBase;
        if (LastAttribute != null)
        {
            LastAttribute.transform.DOScale(new Vector3(1,1,1),0.1f).SetEase(Ease.Linear);
        }
        if(CurrentAttribute != null)
        {
            CurrentAttribute.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.1f).SetEase(Ease.Linear);
        }
    }
}
