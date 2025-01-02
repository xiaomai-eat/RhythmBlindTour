using Qf.Models.AudioEdit;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueryAudioEditAudioClipLength : AbstractQuery<float>
{
    AudioEditModel audioEditModel;
    protected override float OnDo()
    {
        audioEditModel = this.GetModel<AudioEditModel>();
        if(audioEditModel.EditAudioClip != null)
        {
            return audioEditModel.EditAudioClip.length;
        }
        return 0;
    }
}
