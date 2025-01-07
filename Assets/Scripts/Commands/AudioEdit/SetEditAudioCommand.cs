using Qf.Models.AudioEdit;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetEditAudioCommand : AbstractCommand
{
    AudioClip clip;
    public SetEditAudioCommand(AudioClip audioClip)
    {
        clip = audioClip;
    }
    protected override void OnExecute()
    {
        this.GetModel<AudioEditModel>().EditAudioClip = clip;
    }
}
