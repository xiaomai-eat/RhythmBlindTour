using Qf.Models.AudioEdit;
using QFramework;
using System.Collections;
using UnityEngine;

namespace Qf.Commands.AudioEdit
{
    public class SetAudioEditSucceedAudioCommand : AbstractCommand
    {
        AudioEditModel editModel;
        AudioClip audioClip;
        public SetAudioEditSucceedAudioCommand(AudioClip value)
        {
            audioClip = value;
        }
        protected override void OnExecute()
        {
            editModel = this.GetModel<AudioEditModel>();
            editModel.SucceedAudioClip = audioClip;
        }
    }
}